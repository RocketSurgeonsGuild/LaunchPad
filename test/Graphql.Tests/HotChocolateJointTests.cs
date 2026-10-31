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

        executor.Schema.ToString().ShouldContain("type OuterInner");
    }

    [Test]
    public async Task Configures_The_Request_Executor_Setup_With_The_Context_Data_Options()
    {
        var services = new ServiceCollection();
        new HotChocolateJoint().Register(await JointContext.Create(), services);

        services
           .Where(x => x.ServiceType == typeof(IConfigureOptions<RequestExecutorSetup>))
           .Select(x => x.ImplementationType?.Name)
           .ShouldContain("HotChocolateContextDataConfigureOptions");
    }

    [Test]
    public void Is_Exported_As_An_Application_Joint_Without_Ordering()
    {
        var export = Exports.Joints().Single(x => x.Joint is HotChocolateJoint);

        export.HostType.ShouldBe(DovetailHostType.Undefined);
        export.Dependencies.ShouldBeEmpty();
        export.Category.ToString().ShouldBe(DovetailCategory.Application);
        typeof(HotChocolateJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().ShouldBe(DovetailCategory.Application);
    }

    private static async Task<IRequestExecutor> Executor()
    {
        var serviceCollection = new ServiceCollection();
        var dovetailContext = await JointContext.Create();
        dovetailContext.GetOrAdd(() => serviceCollection.AddGraphQL()).AddQueryType<NestedQuery>();
        new HotChocolateJoint().Register(dovetailContext, serviceCollection);
        return await serviceCollection.BuildServiceProvider().GetRequiredService<IRequestExecutorResolver>().GetRequestExecutorAsync();
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
