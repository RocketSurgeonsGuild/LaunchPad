using System.Reflection;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;


using Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake;
using Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake.Joints;
using Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake.Rivets;

using ISerializer = StrawberryShake.Serialization.ISerializer;

namespace NodaTime.Graphql.Tests;

public class NodaTimeStrawberryShakeJointTests
{
    [Test]
    public async Task Registers_A_Serializer_For_Every_NodaTime_Scalar()
    {
        var serializers = await Serializers(new ServiceCollection());

        serializers.Select(x => (x.GetType(), x.TypeName)).ShouldBe(
            [
                (typeof(LocalDateSerializer), "LocalDate"),
                (typeof(LocalTimeSerializer), "LocalTime"),
                (typeof(LocalDateTimeSerializer), "LocalDateTime"),
                (typeof(OffsetDateTimeSerializer), "DateTime"),
                (typeof(DurationSerializer), "Duration"),
            ],
            ignoreOrder: true
        );
    }

    [Test]
    public async Task Registers_The_Serializers_As_Singletons()
    {
        var services = new ServiceCollection();
        new NodaTimeStrawberryShakeJoint().Register(await JointContext.Create(), services);

        services.Where(x => x.ServiceType == typeof(ISerializer)).ShouldAllBe(x => x.Lifetime == ServiceLifetime.Singleton);
    }

    [Test]
    public async Task Resolves_Every_Serializer_With_The_Imported_Joints()
    {
        // the imported joints, as an application gets them
        var context = await DovetailContext.FromAsync(Imports.Joints());
        var services = new ServiceCollection();
        await services.ApplyService(context);

        services.BuildServiceProvider().GetServices<ISerializer>().Count().ShouldBe(5);
    }

    [Test]
    public void Is_Exported_As_A_Core_Joint_Without_Ordering_Or_Gating()
    {
        var export = NodaTimeStrawberryShakeExports.Joints().Single(x => x.Joint is NodaTimeStrawberryShakeJoint);

        export.HostType.ShouldBe(DovetailHostType.Undefined);
        export.Dependencies.ShouldBeEmpty();
        export.Category.ToString().ShouldBe(DovetailCategory.Core);
        typeof(NodaTimeStrawberryShakeJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().ShouldBe(DovetailCategory.Core);
    }

    private static async Task<List<ISerializer>> Serializers(IServiceCollection services)
    {
        new NodaTimeStrawberryShakeJoint().Register(await JointContext.Create(), services);
        return [.. services.BuildServiceProvider().GetServices<ISerializer>()];
    }
}
