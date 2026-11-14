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

        await serializers.Select(x => (x.GetType(), x.TypeName)).Should().BeEquivalentTo([
                (typeof(LocalDateSerializer), "LocalDate"),
                (typeof(LocalTimeSerializer), "LocalTime"),
                (typeof(LocalDateTimeSerializer), "LocalDateTime"),
                (typeof(OffsetDateTimeSerializer), "DateTime"),
                (typeof(DurationSerializer), "Duration"),
            ]);
    }

    [Test]
    public async Task Registers_The_Serializers_As_Singletons()
    {
        var services = new ServiceCollection();
        new NodaTimeStrawberryShakeJoint().Register(await JointContext.Create(), services);

        await services.Where(x => x.ServiceType == typeof(ISerializer)).Should().All(x => x.Lifetime == ServiceLifetime.Singleton);
    }

    [Test]
    public async Task Resolves_Every_Serializer_With_The_Imported_Joints()
    {
        // the imported joints, as an application gets them
        var context = await DovetailContext.FromAsync(Imports.Joints());
        var services = new ServiceCollection();
        await services.ApplyService(context);

        await services.BuildServiceProvider().GetServices<ISerializer>().Count().Should().BeEqualTo(5);
    }

    [Test]
    public async Task Is_Exported_As_A_Core_Joint_Without_Ordering_Or_Gating()
    {
        var export = NodaTimeStrawberryShakeExports.Joints().Single(x => x.Joint is NodaTimeStrawberryShakeJoint);

        await export.HostType.Should().BeEqualTo(DovetailHostType.Undefined);
        await export.Dependencies.Should().BeEmpty();
        await export.Category.ToString().Should().BeEqualTo(DovetailCategory.Core);
        await typeof(NodaTimeStrawberryShakeJoint).GetCustomAttribute<DovetailCategoryAttribute>()!.Category.ToString().Should().BeEqualTo(DovetailCategory.Core);
    }

    private static async Task<List<ISerializer>> Serializers(IServiceCollection services)
    {
        new NodaTimeStrawberryShakeJoint().Register(await JointContext.Create(), services);
        return [.. services.BuildServiceProvider().GetServices<ISerializer>()];
    }
}
