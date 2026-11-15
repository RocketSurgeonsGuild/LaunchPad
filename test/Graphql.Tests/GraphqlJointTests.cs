using System.Reflection;
using HotChocolate.Execution;
using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.HotChocolate;
using Rocket.Surgery.LaunchPad.HotChocolate.Joints;
using Rocket.Surgery.LaunchPad.HotChocolate.Rivets;

namespace Graphql.Tests;

public class GraphqlJointTests
{
    [Test]
    public async Task Gives_An_Unexpected_Exception_Its_Message_And_The_Unknown_Code()
    {
        var executor = await Executor();

        var result = ( await executor.ExecuteAsync("{ missing }") ).ExpectOperationResult();

        // a failure an operation reports is a union response, which the generated mutations answer with its own code; an exception is unexpected
        var error = await result.Errors.Should().HaveSingleItem();
        await error.Message.Should().BeEqualTo("nothing here");
        await error.Code.Should().BeEqualTo("UNKNOWN");
    }

    // Before it became a joint, this wiring took FoundationOptions and an empty RocketChocolateOptions in its constructor but never read them,
    // so the options a context carries change nothing.
    [Test]
    public async Task Registers_The_Same_Services_Whatever_Foundation_Options_The_Context_Has()
    {
        var withoutOptions = new ServiceCollection();
        await Register(withoutOptions);

        var withOptions = new ServiceCollection();
        await Register(
            withOptions,
            context => context.Set(new FoundationOptions { ValidatorLifetime = ServiceLifetime.Transient }).Set(new RocketChocolateOptions())
        );

        await withOptions.Select(x => (x.ServiceType, x.Lifetime)).Should().BeEquivalentTo(withoutOptions.Select(x => (x.ServiceType, x.Lifetime)), CollectionOrdering.Matching);
    }

    [Test]
    public async Task Is_Exported_To_Run_Before_The_Hot_Chocolate_Joint()
    {
        var export = Exports.Joints().Single(x => x.Joint is GraphqlJoint);

        await export.HostType.Should().BeEqualTo(DovetailHostType.Undefined);
        var x = await export.Dependencies.Should().HaveSingleItem();
        await x.Direction.Should().BeEqualTo(DependencyDirection.DependentOf);
        await x.Type.Should().BeEqualTo(typeof(HotChocolateJoint));
        await export.Category.ToString().Should().BeEqualTo(DovetailCategory.Application);
        await typeof(GraphqlJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().Should().BeEqualTo(DovetailCategory.Application);
    }

    [Test]
    public async Task Runs_Before_The_Hot_Chocolate_Joint_Whatever_The_Export_Order()
    {
        var context = await DovetailContext.FromAsync(
            DovetailContextBuilder.Create(
                [.. Exports.Joints().Reverse()],
                new Dictionary<object, object>(),
                DovetailHostType.Live,
                []
            )
        );

        var order = context.Joints.Select(x => x.GetType()).ToList();

        await order.IndexOf(typeof(GraphqlJoint)).Should().BeLessThan(order.IndexOf(typeof(HotChocolateJoint)));
    }

    private static async Task Register(IServiceCollection services, Action<IDovetailContext>? configure = null)
    {
        var context = await JointContext.Create(configure);
        new GraphqlJoint().Register(context, services);
    }

    private static async Task<IRequestExecutor> Executor()
    {
        var services = new ServiceCollection();
        var context = await JointContext.Create();
        context.GetOrAdd(() => services.AddGraphQL()).AddQueryType<Query>();
        new GraphqlJoint().Register(context, services);
        return await services.BuildServiceProvider().GetRequiredService<IRequestExecutorProvider>().GetExecutorAsync();
    }

    public class Query
    {
        public string Missing() => throw new InvalidOperationException("nothing here");
    }
}
