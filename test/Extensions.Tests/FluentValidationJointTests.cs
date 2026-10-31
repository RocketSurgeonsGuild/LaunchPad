using Joinery.Dovetail;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.Foundation.Joints;

namespace Extensions.Tests;

public class FluentValidationJointTests() : LoggerTest<TestRecord>(TestRecord.Create())
{
    [Test]
    public async Task Should_Run_After_The_MediatR_Joint()
    {
        var context = await DovetailContext.FromAsync(TestJoints.CreateBuilder(), TestContext.CancellationToken);

        var joints = context.Joints.Select(z => z.GetType()).ToList();

        joints.ShouldContain(typeof(MediatRJoint));
        joints.ShouldContain(typeof(FluentValidationJoint));
        joints.IndexOf(typeof(FluentValidationJoint)).ShouldBeGreaterThan(joints.IndexOf(typeof(MediatRJoint)));
    }

    [Test]
    [Arguments(ServiceLifetime.Singleton)]
    [Arguments(ServiceLifetime.Scoped)]
    [Arguments(ServiceLifetime.Transient)]
    public async Task Should_Read_The_Mediator_Lifetime_From_The_Foundation_Options_In_The_Context(ServiceLifetime lifetime)
    {
        var builder = TestJoints.CreateBuilder().Set(new FoundationOptions { MediatorLifetime = lifetime, });
        var context = await DovetailContext.FromAsync(builder, TestContext.CancellationToken);
        var services = new ServiceCollection();

        new FluentValidationJoint().Register(context, services);

        services.ShouldContain(
            x => x.ServiceType == typeof(IPipelineBehavior<,>) && x.ImplementationType!.Name.StartsWith("ValidationPipelineBehavior", StringComparison.Ordinal) && x.Lifetime == lifetime
        );
        services.ShouldContain(
            x => x.ServiceType == typeof(IStreamPipelineBehavior<,>) && x.ImplementationType!.Name.StartsWith("ValidationStreamPipelineBehavior", StringComparison.Ordinal) && x.Lifetime == lifetime
        );
    }
}
