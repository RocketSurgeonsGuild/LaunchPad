using System.Reflection;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;

using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.Foundation.Joints;
using Rocket.Surgery.LaunchPad.Foundation.Rivets;

namespace FluentValidation.Tests;

public class FluentValidationJointTests() : LoggerTest<TestRecord>(TestRecord.Create())
{
    [Test]
    public async Task Is_Exported_As_A_Core_Joint_With_No_Dependencies()
    {
        var export = FluentValidationExports.Joints().Single(x => x.Joint is FluentValidationJoint);

        await export.HostType.Should().BeEqualTo(DovetailHostType.Undefined);
        await export.Dependencies.Should().BeEmpty();
        await export.Category.ToString().Should().BeEqualTo(DovetailCategory.Core);
        await typeof(FluentValidationJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().Should().BeEqualTo(DovetailCategory.Core);
    }

    [Test]
    public async Task Should_Register_The_Options_Validation()
    {
        var context = await DovetailContext.FromAsync(TestJoints.CreateBuilder(), TestContext.CancellationToken);
        var services = new ServiceCollection();

        context.Joints.OfType<FluentValidationJoint>().Single().Register(context, services);

        await services.Should().Contain(x => x.ServiceType == typeof(Microsoft.Extensions.Options.IValidateOptions<>));
    }
}
