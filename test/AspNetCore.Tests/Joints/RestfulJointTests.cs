using Joinery.Dovetail.Joints;

using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.AspNetCore.Composition;
using Rocket.Surgery.LaunchPad.AspNetCore.Joints;

namespace AspNetCore.Tests.Joints;

public class RestfulJointTests
{
    [Test]
    public async Task Should_Register_The_Restful_Api_Application_Model_Provider()
    {
        var services = await JointTestContext.ApplyServicesAsync<RestfulJoint>();

        services.ShouldContain(
            d => d.ServiceType == typeof(IApplicationModelProvider)
             && d.ImplementationType == typeof(RestfulApiApplicationModelProvider)
             && d.Lifetime == ServiceLifetime.Transient
        );
    }

    [Test]
    public async Task Should_Register_Controllers_As_Services()
    {
        var services = await JointTestContext.ApplyServicesAsync<RestfulJoint>();

        services.ShouldContain(d => d.ServiceType == typeof(IControllerActivator) && d.ImplementationType == typeof(ServiceBasedControllerActivator));
    }

    [Test]
    public async Task Should_Not_Register_The_Model_Provider_Twice_When_The_Joint_Is_Applied_Twice()
    {
        var context = await JointTestContext.CreateBuilder().AppendJoint<RestfulJoint>().CreateAsync();
        var services = new ServiceCollection();

        await services.ApplyService(context);
        await services.ApplyService(context);

        services.Count(d => d.ImplementationType == typeof(RestfulApiApplicationModelProvider)).ShouldBe(1);
    }
}
