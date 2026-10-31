using System.Reflection;

using Grpc.AspNetCore.Server;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.Grpc;
using Rocket.Surgery.LaunchPad.Grpc.Joints;

namespace Grpc.Tests.Joints;

public class GrpcJointTests
{
    [Test]
    public async Task Should_Add_The_Exception_Interceptors_In_Order()
    {
        using var provider = await BuildAsync();

        var interceptors = provider
                          .GetRequiredService<IOptions<GrpcServiceOptions>>()
                          .Value
                          .Interceptors
                          .Select(z => z.Type)
                          .ToArray();

        interceptors.ShouldBe(
            [
                typeof(NotAuthorizedInterceptor),
                typeof(NotFoundInterceptor),
                typeof(RequestFailedInterceptor),
            ]
        );
    }

    [Test]
    public void Should_Be_An_Application_Joint_That_Runs_In_Every_Host_Type()
    {
        typeof(GrpcJoint).GetCustomAttribute<DovetailCategoryAttribute>().ShouldNotBeNull().Category.ToString().ShouldBe(DovetailCategory.Application);
        Attribute.GetCustomAttributes(typeof(GrpcJoint)).ShouldNotContain(z => z is LiveJointAttribute || z is UnitTestJointAttribute || z is DovetailHostTypeAttribute);
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
