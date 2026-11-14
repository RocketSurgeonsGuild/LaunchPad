using System.Reflection;
using HotChocolate.Execution;
using HotChocolate.Execution.Configuration;
using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Rocket.Surgery.LaunchPad.HotChocolate.Joints;
using Rocket.Surgery.LaunchPad.HotChocolate.Rivets;

namespace Graphql.Tests;

public class HotChocolateJointTests
{
    [Test]
    public async Task Names_Nested_Types_After_Their_Declaring_Type()
    {
        var executor = await Executor();

        await executor.Schema.ToString().Should().Contain("type OuterInner");
    }

    [Test]
    public async Task Configures_The_Request_Executor_Setup_With_The_Context_Data_Options()
    {
        var services = new ServiceCollection();
        new HotChocolateJoint().Register(await JointContext.Create(), services);

        await services
           .Where(x => x.ServiceType == typeof(IConfigureOptions<RequestExecutorSetup>))
           .Select(x => x.ImplementationType?.Name)
           .Should().Contain("HotChocolateContextDataConfigureOptions");
    }

    [Test]
    public async Task Is_Exported_As_An_Application_Joint_Without_Ordering()
    {
        var export = Exports.Joints().Single(x => x.Joint is HotChocolateJoint);

        await export.HostType.Should().BeEqualTo(DovetailHostType.Undefined);
        await export.Dependencies.Should().BeEmpty();
        await export.Category.ToString().Should().BeEqualTo(DovetailCategory.Application);
        await typeof(HotChocolateJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().Should().BeEqualTo(DovetailCategory.Application);
    }

    private static async Task<IRequestExecutor> Executor()
    {
        var serviceCollection = new ServiceCollection();
        var dovetailContext = await JointContext.Create();
        dovetailContext.GetOrAdd(() => serviceCollection.AddGraphQL()).AddQueryType<NestedQuery>();
        new HotChocolateJoint().Register(dovetailContext, serviceCollection);
        return await serviceCollection.BuildServiceProvider().GetRequiredService<IRequestExecutorProvider>().GetExecutorAsync();
    }
}

public class NestedQuery
{
    public Outer.Inner Nested() => new();
}

public static class Outer
{
    public class Inner
    {
        public string Value => "inner";
    }
}
