using System.Reflection;

using FakeItEasy;

using Grpc.AspNetCore.Server;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.Grpc;
using Rocket.Surgery.LaunchPad.Grpc.Joints;
using Rocket.Surgery.LaunchPad.Grpc.Validation;

namespace Grpc.Tests.Joints;

public class GrpcJointTests
{
    [Test]
    public async Task Should_Add_The_Validation_And_Exception_Interceptors_In_Order()
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
                typeof(ValidationInterceptor),
                typeof(NotAuthorizedInterceptor),
                typeof(NotFoundInterceptor),
                typeof(RequestFailedInterceptor),
            ]
        );
    }

    [Test]
    public async Task Should_Register_The_Default_Validator_Error_Message_Handler()
    {
        using var provider = await BuildAsync();

        provider.GetRequiredService<IValidatorErrorMessageHandler>().ShouldBeOfType<DefaultErrorMessageHandler>();
    }

    [Test]
    public async Task Should_Keep_The_Validator_Error_Message_Handler_That_Was_Already_Registered()
    {
        var context = await CreateContextAsync();
        var handler = A.Fake<IValidatorErrorMessageHandler>();
        var services = new ServiceCollection();
        services.AddSingleton(handler);

        await services.ApplyService(context);
        using var provider = services.AddLogging().BuildServiceProvider();

        provider.GetRequiredService<IValidatorErrorMessageHandler>().ShouldBeSameAs(handler);
    }

    [Test]
    public void Should_Be_An_Application_Joint_That_Runs_In_Every_Host_Type()
    {
        typeof(GrpcJoint).GetCustomAttribute<DovetailCategoryAttribute>().ShouldNotBeNull().Category.ToString().ShouldBe(DovetailCategory.Application);
        Attribute.GetCustomAttributes(typeof(GrpcJoint)).ShouldNotContain(z => z is LiveJointAttribute || z is UnitTestJointAttribute || z is DovetailHostTypeAttribute);
    }

    private static async Task<IDovetailContext> CreateContextAsync() => await DovetailContextBuilder
                                                                              .Create(
                                                                                   [],
                                                                                   new Dictionary<object, object>(),
                                                                                   [DovetailCategory.Core, DovetailCategory.Application]
                                                                               )
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
