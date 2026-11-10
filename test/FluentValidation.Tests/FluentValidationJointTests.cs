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
    public void Is_Exported_As_A_Core_Joint_With_No_Dependencies()
    {
        var export = FluentValidationExports.Joints().Single(x => x.Joint is FluentValidationJoint);

        export.HostType.ShouldBe(DovetailHostType.Undefined);
        export.Dependencies.ShouldBeEmpty();
        export.Category.ToString().ShouldBe(DovetailCategory.Core);
        typeof(FluentValidationJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().ShouldBe(DovetailCategory.Core);
    }

    [Test]
    public async Task Should_Register_The_Options_Validation()
    {
        var context = await DovetailContext.FromAsync(TestJoints.CreateBuilder(), TestContext.CancellationToken);
        var services = new ServiceCollection();

        context.Joints.OfType<FluentValidationJoint>().Single().Register(context, services);

        services.ShouldContain(x => x.ServiceType == typeof(Microsoft.Extensions.Options.IValidateOptions<>));
    }
}
