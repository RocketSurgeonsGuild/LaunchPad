using System.Reflection;

using Grpc.AspNetCore.Server;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.Grpc.Joints;

namespace Grpc.Tests.Joints;

public class GrpcJointTests
{
    [Test]
    public async Task Should_Register_Grpc_Without_Exception_Interceptors()
    {
        using var provider = await BuildAsync();

        var interceptors = provider
                          .GetRequiredService<IOptions<GrpcServiceOptions>>()
                          .Value
                          .Interceptors
                          .Select(z => z.Type)
                          .ToArray();

        // an operation reports a failure with a union response, which the generated gRPC servers answer with its status
        await interceptors.Should().BeEmpty();
    }

    [Test]
    public async Task Should_Be_An_Application_Joint_That_Runs_In_Every_Host_Type()
    {
        await ( await typeof(GrpcJoint).GetCustomAttribute<DovetailCategoryAttribute>().Should().NotBeNull() )!.Category.ToString().Should().BeEqualTo(DovetailCategory.Application);
        await Attribute.GetCustomAttributes(typeof(GrpcJoint)).Should().All(z => z is not ( LiveJointAttribute or UnitTestJointAttribute or DovetailHostTypeAttribute ));
    }

    // Dovetail resolves joints whose category is not in the context's category set, so the set stays empty to run every joint.
    private static async Task<IDovetailContext> CreateContextAsync() => await DovetailContextBuilder
                                                                              .Create([], new Dictionary<object, object>(), [])
                                                                              .AppendJoint<GrpcJoint>()
                                                                              .CreateAsync();

    private static async Task<ServiceProvider> BuildAsync()
    {
        var context = await CreateContextAsync();
        var services = new ServiceCollection();
        await services.ApplyService(context);
        return services.AddLogging().BuildServiceProvider();
    }
}
