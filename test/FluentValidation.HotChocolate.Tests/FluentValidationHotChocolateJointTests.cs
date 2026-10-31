using System.Reflection;

using FairyBread;

using global::HotChocolate.Execution;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Infrastructure;

using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.HotChocolate.Joints;
using Rocket.Surgery.LaunchPad.HotChocolate.Rivets;

namespace FluentValidation.HotChocolate.Tests;

public class FluentValidationHotChocolateJointTests
{
    [Test]
    public async Task Replaces_The_Fairy_Bread_Validator_Registry()
    {
        var services = new ServiceCollection();
        await Register(services);

        var descriptor = services.Single(x => x.ServiceType == typeof(IValidatorRegistry));
        descriptor.ImplementationType!.Name.ShouldBe("LaunchPadValidatorRegistry");
        descriptor.Lifetime.ShouldBe(ServiceLifetime.Singleton);
    }

    [Test]
    public async Task Finds_The_Validators_By_The_Type_They_Validate()
    {
        var services = new ServiceCollection().AddSingleton<IValidator>(new InlineValidator<Request>());
        await Register(services);

        var registry = services.BuildServiceProvider().GetRequiredService<IValidatorRegistry>();

        registry.Cache.Keys.ShouldBe([typeof(Request)]);
        registry.Cache[typeof(Request)].Select(x => x.ValidatorType).ShouldBe([typeof(InlineValidator<Request>)]);
    }

    [Test]
    public async Task Validates_Arguments_With_The_Registered_Validators()
    {
        var executor = await Executor(
            services => services.AddTransient<RequestValidator>().AddTransient<IValidator>(sp => sp.GetRequiredService<RequestValidator>())
        );

        var valid = ( await executor.ExecuteAsync("""{ echo(request: { name: "a" }) }""") ).ExpectOperationResult();
        var invalid = ( await executor.ExecuteAsync("""{ echo(request: { name: "" }) }""") ).ExpectOperationResult();

        string.Join("; ", valid.Errors?.Select(x => x.Message) ?? []).ShouldBeEmpty();
        invalid.Errors.ShouldNotBeNull().ShouldNotBeEmpty();
    }

    [Test]
    public async Task Validates_Arguments_When_Added_Before_The_Graphql_Joint()
    {
        var executor = await Executor(
            services => services.AddTransient<RequestValidator>().AddTransient<IValidator>(sp => sp.GetRequiredService<RequestValidator>()),
            fluentValidationFirst: true
        );

        var invalid = ( await executor.ExecuteAsync("""{ echo(request: { name: "" }) }""") ).ExpectOperationResult();

        invalid.Errors.ShouldNotBeNull().ShouldNotBeEmpty();
    }

    [Test]
    public void Is_Exported_To_Run_Before_The_Hot_Chocolate_Joint()
    {
        var export = FluentValidationExports.Joints().Single(x => x.Joint is FluentValidationHotChocolateJoint);

        export.HostType.ShouldBe(DovetailHostType.Undefined);
        export.Dependencies.ShouldHaveSingleItem().ShouldSatisfyAllConditions(
            x => x.Direction.ShouldBe(DependencyDirection.DependentOf),
            x => x.Type.ShouldBe(typeof(HotChocolateJoint))
        );
        typeof(FluentValidationHotChocolateJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().ShouldBe(DovetailCategory.Application);
    }

    private static async Task<IDovetailContext> CreateContext() =>
        await DovetailContext.FromAsync(DovetailContextBuilder.Create([], new Dictionary<object, object>(), []));

    private static async Task Register(IServiceCollection services)
    {
        var context = await CreateContext();
        new FluentValidationHotChocolateJoint().Register(context, services);
    }

    private static async Task<IRequestExecutor> Executor(Action<IServiceCollection>? configure = null, bool fluentValidationFirst = false)
    {
        var services = new ServiceCollection();
        configure?.Invoke(services);
        var context = await CreateContext();
        context.GetOrAdd(() => services.AddGraphQL()).AddQueryType<Query>();
        if (fluentValidationFirst) new FluentValidationHotChocolateJoint().Register(context, services);
        new GraphqlJoint().Register(context, services);
        if (!fluentValidationFirst) new FluentValidationHotChocolateJoint().Register(context, services);
        return await services.BuildServiceProvider().GetRequiredService<IRequestExecutorResolver>().GetRequestExecutorAsync();
    }

    public record Request(string Name);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator() => RuleFor(x => x.Name).NotEmpty();
    }

    public class Query
    {
        public string Echo(Request request) => request.Name;
    }
}
