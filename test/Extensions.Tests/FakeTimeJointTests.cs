using Joinery.Dovetail;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;

using NodaTime;

using Rocket.Surgery.LaunchPad.Primitives.Joints;
using Rocket.Surgery.LaunchPad.Testing;

namespace Extensions.Tests;

public class FakeTimeJointTests() : JointFakeTest()
{
    [Test]
    public async Task Clock_Joint_Default()
    {
        await Init(x => { x.Set(DovetailHostType.UnitTest); });
        var clock = Container.GetRequiredService<IClock>();
        clock.ShouldBeAssignableTo<IClock>();
        clock.GetCurrentInstant().ShouldBe(Instant.FromUnixTimeSeconds(1577836800));
        clock.GetCurrentInstant().ShouldBe(Instant.FromUnixTimeSeconds(1577836800) + Duration.FromSeconds(1));
    }

    [Test]
    public async Task Clock_Joint_Override()
    {
        await Init(x => { x.Set(new FakeTimeOptions { UnixTimeSeconds = 0, AdvanceBy = Duration.FromMinutes(1), }); });

        var clock = ServiceProvider.GetRequiredService<IClock>();
        clock.ShouldBeAssignableTo<IClock>();
        clock.GetCurrentInstant().ShouldBe(Instant.FromUnixTimeSeconds(0));
        clock.GetCurrentInstant().ShouldBe(Instant.FromUnixTimeSeconds(0) + Duration.FromMinutes(1));
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

        ( provider.GetService<FakeTimeProvider>() is { } ).ShouldBe(applies);
        provider.GetRequiredService<TimeProvider>().ShouldBe(applies ? provider.GetRequiredService<FakeTimeProvider>() : TimeProvider.System);
    }

    [Test]
    public async Task Fake_Time_Joint_Runs_Before_The_Time_Joint()
    {
        var context = await DovetailContext.FromAsync(TestJoints.CreateBuilder());

        var joints = context.Joints.Select(z => z.GetType()).ToList();

        joints.ShouldContain(typeof(FakeTimeJoint));
        joints.ShouldContain(typeof(TimeJoint));
        joints.IndexOf(typeof(FakeTimeJoint)).ShouldBeLessThan(joints.IndexOf(typeof(TimeJoint)));
    }
}
