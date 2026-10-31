using System.Reflection;
using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.LaunchPad.StrawberryShake;
using Rocket.Surgery.LaunchPad.StrawberryShake.Joints;
using Rocket.Surgery.LaunchPad.StrawberryShake.Rivets;
using ISerializer = StrawberryShake.Serialization.ISerializer;

namespace Graphql.Tests;

public class StrawberryShakeJointTests
{
    [Test]
    public async Task Registers_The_NodaTime_Serializers_As_Singletons()
    {
        var services = new ServiceCollection();
        new StrawberryShakeJoint().Register(await JointContext.Create(), services);

        services.Where(x => x.ServiceType == typeof(ISerializer)).Select(x => (x.ImplementationType, x.Lifetime)).ShouldBe(
            [
                (typeof(InstantSerializer), ServiceLifetime.Singleton),
                (typeof(LocalDateSerializer), ServiceLifetime.Singleton),
                (typeof(LocalTimeSerializer), ServiceLifetime.Singleton),
                (typeof(LocalDateTimeSerializer), ServiceLifetime.Singleton),
                (typeof(OffsetDateTimeSerializer), ServiceLifetime.Singleton),
                (typeof(OffsetTimeSerializer), ServiceLifetime.Singleton),
                (typeof(PeriodSerializer), ServiceLifetime.Singleton),
                (typeof(DurationSerializer), ServiceLifetime.Singleton),
                (typeof(ZonedDateTimeSerializer), ServiceLifetime.Singleton),
                (typeof(OffsetSerializer), ServiceLifetime.Singleton),
                (typeof(IsoDayOfWeekSerializer), ServiceLifetime.Singleton),
            ]
        );
    }

    [Test]
    public void Is_Exported_As_A_Core_Joint_Without_Ordering_Or_Gating()
    {
        var export = Exports.Joints().Single(x => x.Joint is StrawberryShakeJoint);

        export.HostType.ShouldBe(DovetailHostType.Undefined);
        export.Dependencies.ShouldBeEmpty();
        // The generated export metadata does not carry the category in Dovetail 0.3.0, so the attribute is what declares it.
        typeof(StrawberryShakeJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().ShouldBe(DovetailCategory.Core);
    }
}
