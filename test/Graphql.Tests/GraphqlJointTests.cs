using System.Reflection;
using FairyBread;
using FluentValidation;
using HotChocolate.Execution;
using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Infrastructure;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.HotChocolate;
using Rocket.Surgery.LaunchPad.HotChocolate.Joints;
using Rocket.Surgery.LaunchPad.HotChocolate.Rivets;
using Rocket.Surgery.LaunchPad.NodaTime.Joints;
using Rocket.Surgery.LaunchPad.Primitives;

namespace Graphql.Tests;

public class GraphqlJointTests
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
    public async Task Binds_The_Mediatr_Unit_To_The_Void_Scalar()
    {
        var executor = await Executor();

        executor.Schema.ToString().ShouldContain("scalar Void");
    }

    [Test]
    public async Task Gives_Not_Found_Exceptions_The_Notfound_Code()
    {
        var executor = await Executor();

        var result = ( await executor.ExecuteAsync("{ missing }") ).ExpectOperationResult();

        result.Errors.ShouldHaveSingleItem().Code.ShouldBe("NOTFOUND");
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

    // The convention took FoundationOptions and an empty RocketChocolateOptions in its constructor but never read them,
    // so the options a context carries change nothing.
    [Test]
    public async Task Registers_The_Same_Services_Whatever_Foundation_Options_The_Context_Has()
    {
        var withoutOptions = new ServiceCollection();
        await Register(withoutOptions);

        var withOptions = new ServiceCollection();
        await Register(
            withOptions,
            context => context.Set(new FoundationOptions { MediatorLifetime = ServiceLifetime.Singleton, ValidatorLifetime = ServiceLifetime.Transient }).Set(new RocketChocolateOptions())
        );

        withOptions.Select(x => (x.ServiceType, x.Lifetime)).ShouldBe(withoutOptions.Select(x => (x.ServiceType, x.Lifetime)));
    }

    [Test]
    public void Is_Exported_To_Run_Before_The_Hot_Chocolate_Joint()
    {
        var export = Exports.Joints().Single(x => x.Joint is GraphqlJoint);

        export.HostType.ShouldBe(DovetailHostType.Undefined);
        export.Dependencies.ShouldHaveSingleItem().ShouldSatisfyAllConditions(
            x => x.Direction.ShouldBe(DependencyDirection.DependentOf),
            x => x.Type.ShouldBe(typeof(HotChocolateJoint))
        );
        export.Category.ToString().ShouldBe(DovetailCategory.Application);
        typeof(GraphqlJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().ShouldBe(DovetailCategory.Application);
    }

    [Test]
    public async Task Runs_Before_The_Hot_Chocolate_Joint_And_After_The_Time_Joint_Whatever_The_Export_Order()
    {
        var context = await DovetailContext.FromAsync(
            DovetailContextBuilder.Create(
                [
                    ..Exports.Joints().Reverse(),
                    new DovetailJointMetadata(new TimeJoint(), DovetailHostType.Undefined),
                ],
                new Dictionary<object, object>(),
                DovetailHostType.Live,
                []
            )
        );

        var order = context.Joints.Select(x => x.GetType()).ToList();

        order.IndexOf(typeof(GraphqlJoint)).ShouldBeLessThan(order.IndexOf(typeof(HotChocolateJoint)));
        order.IndexOf(typeof(TimeJoint)).ShouldBeLessThan(order.IndexOf(typeof(HotChocolateJoint)));
    }

    private static async Task Register(IServiceCollection services, Action<IDovetailContext>? configure = null)
    {
        var context = await JointContext.Create(configure);
        new GraphqlJoint().Register(context, services);
    }

    private static async Task<IRequestExecutor> Executor(Action<IServiceCollection>? configure = null)
    {
        var services = new ServiceCollection();
        configure?.Invoke(services);
        var context = await JointContext.Create();
        context.GetOrAdd(() => services.AddGraphQL()).AddQueryType<Query>();
        new GraphqlJoint().Register(context, services);
        return await services.BuildServiceProvider().GetRequiredService<IRequestExecutorResolver>().GetRequestExecutorAsync();
    }

    public record Request(string Name);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator() => RuleFor(x => x.Name).NotEmpty();
    }

    public class Query
    {
        public string Missing() => throw new NotFoundException("nothing here");

        public Unit Nothing() => Unit.Value;

        public string Echo(Request request) => request.Name;
    }
}
