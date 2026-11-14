using Joinery.Dovetail;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;

using Rocket.Surgery.LaunchPad.NodaTime.Joints;

namespace NodaTime.Tests;

public class FakeTimeJointTests() : JointTest()
{
    [Test]
    public async Task Clock_Joint_Default()
    {
        await Init(x => { x.Set(DovetailHostType.UnitTest); });
        var clock = Container.GetRequiredService<IClock>();
        await Assert.That(clock).IsAssignableTo<IClock>();
        await clock.GetCurrentInstant().Should().BeEqualTo(Instant.FromUnixTimeSeconds(1577836800));
        await clock.GetCurrentInstant().Should().BeEqualTo(Instant.FromUnixTimeSeconds(1577836800) + Duration.FromSeconds(1));
    }

    [Test]
    public async Task Clock_Joint_Override()
    {
        await Init(x => { x.Set(new FakeTimeOptions { UnixTimeSeconds = 0, AdvanceBy = Duration.FromMinutes(1), }); });

        var clock = ServiceProvider.GetRequiredService<IClock>();
        await Assert.That(clock).IsAssignableTo<IClock>();
        await clock.GetCurrentInstant().Should().BeEqualTo(Instant.FromUnixTimeSeconds(0));
        await clock.GetCurrentInstant().Should().BeEqualTo(Instant.FromUnixTimeSeconds(0) + Duration.FromMinutes(1));
    }

    [Test]
    [Arguments(DovetailHostType.UnitTest, true)]
    [Arguments(DovetailHostType.Live, false)]
    public async Task Fake_Time_Joint_Only_Applies_To_Unit_Tests(DovetailHostType hostType, bool applies)
    {
        var builder = DovetailContextBuilder.Create([], new Dictionary<object, object>(), hostType, []);
        builder.AppendJoint(new FakeTimeJoint());
        builder.AppendJoint<TimeJoint>();

        var services = await TestJoints.ApplyServicesAsync(await DovetailContext.FromAsync(builder));
        var provider = services.BuildServiceProvider();

        await ( provider.GetService<FakeTimeProvider>() is { } ).Should().BeEqualTo(applies);
        await provider.GetRequiredService<TimeProvider>().Should().BeEqualTo(applies ? provider.GetRequiredService<FakeTimeProvider>() : TimeProvider.System);
    }

    [Test]
    public async Task Fake_Time_Joint_Runs_Before_The_Time_Joint()
    {
        var context = await DovetailContext.FromAsync(TestJoints.CreateBuilder());

        var joints = context.Joints.Select(z => z.GetType()).ToList();

        await joints.Should().Contain(typeof(FakeTimeJoint));
        await joints.Should().Contain(typeof(TimeJoint));
        await joints.IndexOf(typeof(FakeTimeJoint)).Should().BeLessThan(joints.IndexOf(typeof(TimeJoint)));
    }
}
