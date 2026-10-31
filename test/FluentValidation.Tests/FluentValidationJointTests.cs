using System.Reflection;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Infrastructure;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.Foundation.Joints;
using Rocket.Surgery.LaunchPad.Foundation.Rivets;

namespace FluentValidation.Tests;

public class FluentValidationJointTests() : LoggerTest<TestRecord>(TestRecord.Create())
{
    [Test]
    public void Is_Exported_As_A_Core_Joint_That_Runs_After_The_MediatR_Joint()
    {
        var export = FluentValidationExports.Joints().Single(x => x.Joint is FluentValidationJoint);

        export.HostType.ShouldBe(DovetailHostType.Undefined);
        export.Dependencies.ShouldHaveSingleItem().ShouldSatisfyAllConditions(
            x => x.Direction.ShouldBe(DependencyDirection.DependsOn),
            x => x.Type.ShouldBe(typeof(MediatRJoint))
        );
        export.Category.ToString().ShouldBe(DovetailCategory.Core);
        typeof(FluentValidationJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().ShouldBe(DovetailCategory.Core);
    }

    [Test]
    public async Task Runs_After_The_MediatR_Joint_Whatever_The_Export_Order()
    {
        var context = await DovetailContext.FromAsync(
            DovetailContextBuilder.Create(
                [
                    ..FluentValidationExports.Joints().Reverse(),
                    new DovetailJointMetadata(new MediatRJoint(), DovetailHostType.Undefined),
                ],
                new Dictionary<object, object>(),
                DovetailHostType.Live,
                []
            ),
            TestContext.CancellationToken
        );

        var order = context.Joints.Select(x => x.GetType()).ToList();

        order.IndexOf(typeof(FluentValidationJoint)).ShouldBeGreaterThan(order.IndexOf(typeof(MediatRJoint)));
    }

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

        // the joint Dovetail created from the context, which resolved the options set above
        context.Joints.OfType<FluentValidationJoint>().Single().Register(context, services);

        services.ShouldContain(
            x => x.ServiceType == typeof(IPipelineBehavior<,>) && x.ImplementationType!.Name.StartsWith("ValidationPipelineBehavior", StringComparison.Ordinal) && x.Lifetime == lifetime
        );
        services.ShouldContain(
            x => x.ServiceType == typeof(IStreamPipelineBehavior<,>) && x.ImplementationType!.Name.StartsWith("ValidationStreamPipelineBehavior", StringComparison.Ordinal) && x.Lifetime == lifetime
        );
    }
}
