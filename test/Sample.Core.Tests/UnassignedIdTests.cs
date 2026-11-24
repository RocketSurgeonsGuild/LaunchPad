using Microsoft.Extensions.DependencyInjection;

using NodaTime;

using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Core.Operations.LaunchRecords;
using Sample.Core.Operations.Rockets;

using Serilog.Events;

using ZeroAlloc.Mediator;
using ZeroAlloc.Mediator.Validation;

namespace Sample.Core.Tests;

// The samples' strong IDs accept the empty Guid (RejectEmpty = false), and a struct domain primitive has a default instance with no value at all:
// the IsAssigned rule of every operation that takes an ID rejects both, before the handler looks the ID up.
public class UnassignedIdTests() : HandleTestHostBase(LogEventLevel.Verbose)
{
    [Test]
    [MethodDataSource(nameof(UnassignedIds))]
    public async Task Should_Reject_An_Unassigned_Id(UnassignedIdCase testCase)
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        Func<Task> send = async () => await testCase.Send(scope.ServiceProvider.GetRequiredService<IMediator>(), TestContext.CancellationToken);

        var exception = await send.Should().Throw<ValidationFailedException>();
        await exception.Error.Failures.Select(x => x.PropertyName).Should().Contain(testCase.PropertyName);
    }

    public static IEnumerable<Func<UnassignedIdCase>> UnassignedIds()
    {
        // RocketId.Empty is the default instance of the struct (IsDefault); Create(Guid.Empty) holds the empty Guid.
        (string Kind, RocketId Rocket, LaunchRecordId LaunchRecord)[] kinds =
        [
            ("default", RocketId.Empty, LaunchRecordId.Empty),
            ("Guid.Empty", RocketId.Create(Guid.Empty), LaunchRecordId.Create(Guid.Empty)),
        ];

        foreach (var (kind, rocket, launchRecord) in kinds)
        {
            yield return () => new($"GetRocket ({kind})", nameof(GetRocket.Request.Id), async (m, ct) => await m.Send(new GetRocket.Request(rocket), ct));
            yield return () => new($"DeleteRocket ({kind})", nameof(DeleteRocket.Request.Id), async (m, ct) => await m.Send(new DeleteRocket.Request(rocket), ct));
            yield return () => new(
                $"EditRocket ({kind})",
                nameof(EditRocket.Request.Id),
                async (m, ct) => await m.Send(new EditRocket.Request { Id = rocket, SerialNumber = "12345678901234", Type = RocketType.Falcon9 }, ct)
            );
            yield return () => new(
                $"GetRocketLaunchRecords ({kind})",
                nameof(GetRocketLaunchRecords.Request.Id),
                async (m, ct) =>
                {
                    await foreach (var _ in m.CreateStream(new GetRocketLaunchRecords.Request { Id = rocket }, ct)) { }
                }
            );
            yield return () => new(
                $"GetRocketLaunchRecord rocket ({kind})",
                nameof(GetRocketLaunchRecord.Request.Id),
                async (m, ct) => await m.Send(new GetRocketLaunchRecord.Request { Id = rocket, LaunchRecordId = LaunchRecordId.Create() }, ct)
            );
            yield return () => new(
                $"GetRocketLaunchRecord launch record ({kind})",
                nameof(GetRocketLaunchRecord.Request.LaunchRecordId),
                async (m, ct) => await m.Send(new GetRocketLaunchRecord.Request { Id = RocketId.Create(), LaunchRecordId = launchRecord }, ct)
            );
            yield return () => new(
                $"GetLaunchRecord ({kind})",
                nameof(GetLaunchRecord.Request.Id),
                async (m, ct) => await m.Send(new GetLaunchRecord.Request(launchRecord), ct)
            );
            yield return () => new(
                $"DeleteLaunchRecord ({kind})",
                nameof(DeleteLaunchRecord.Request.Id),
                async (m, ct) => await m.Send(new DeleteLaunchRecord.Request(launchRecord), ct)
            );
            yield return () => new(
                $"CreateLaunchRecord ({kind})",
                nameof(CreateLaunchRecord.Request.RocketId),
                async (m, ct) => await m.Send(
                    new CreateLaunchRecord.Request
                    {
                        RocketId = rocket,
                        Partner = "partner",
                        Payload = "payload",
                        ScheduledLaunchDate = ScheduledLaunchDate,
                    },
                    ct
                )
            );
            yield return () => new(
                $"EditLaunchRecord launch record ({kind})",
                nameof(EditLaunchRecord.Request.Id),
                async (m, ct) => await m.Send(EditLaunchRecordRequest(launchRecord, RocketId.Create()), ct)
            );
            yield return () => new(
                $"EditLaunchRecord rocket ({kind})",
                nameof(EditLaunchRecord.Request.RocketId),
                async (m, ct) => await m.Send(EditLaunchRecordRequest(LaunchRecordId.Create(), rocket), ct)
            );
        }
    }

    private static EditLaunchRecord.Request EditLaunchRecordRequest(LaunchRecordId id, RocketId rocketId) => new()
    {
        Id = id,
        RocketId = rocketId,
        Partner = "partner",
        Payload = "payload",
        ScheduledLaunchDate = ScheduledLaunchDate,
    };

    private static readonly OffsetDateTime ScheduledLaunchDate = OffsetDateTime.FromDateTimeOffset(new(2030, 1, 1, 0, 0, 0, TimeSpan.Zero));

    /// <summary>A request whose ID is not assigned, sent through the mediator.</summary>
    public sealed class UnassignedIdCase
    {
        internal UnassignedIdCase(string name, string propertyName, Func<IMediator, CancellationToken, Task> send)
        {
            Name = name;
            PropertyName = propertyName;
            Send = send;
        }

        public string Name { get; }
        public string PropertyName { get; }
        internal Func<IMediator, CancellationToken, Task> Send { get; }

        public override string ToString() => Name;
    }
}
