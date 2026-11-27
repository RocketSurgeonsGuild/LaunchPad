
using Riok.Mapperly.Abstractions;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Authorization;
using Sample.Core.Domain;
using Sample.Core.Models;
using ZeroAlloc.Authorization;
using ZeroAlloc.Mediator;
using ZeroAlloc.Validation;

namespace Sample.Core.Operations.Rockets;

[PublicAPI]
[Mapper]
[UseStaticMapper(typeof(NodaTimeMapper))]
public static partial class GetRocketLaunchRecord
{
    [Validate]
    [RequirePolicy(Policies.LaunchRecordsRead)]
    public readonly record struct Request : IRequest<Result>
    {
        /// <summary>
        ///     The rocket id
        /// </summary>
        [Must(nameof(IsAssigned))]
        public RocketId Id { get; init; }

        /// <summary>
        ///     The launch record id
        /// </summary>
        [Must(nameof(IsAssigned))]
        public LaunchRecordId LaunchRecordId { get; init; }

        internal bool IsAssigned(RocketId id) => !id.IsDefault && id.Value != Guid.Empty;

        internal bool IsAssigned(LaunchRecordId id) => !id.IsDefault && id.Value != Guid.Empty;
    }

    /// <summary>What the operation answers: the launch record, or not found; unauthorized or forbidden when the caller may not ask.</summary>
    public readonly union Result(LaunchRecordModel, NotFound, Unauthorized, Forbidden);

    internal class GetRocketLaunchRecordHandler(RocketDbContext dbContext) : IRequestHandler<Request, Result>
    {
        public async ValueTask<Result> Handle(Request request, CancellationToken cancellationToken) =>
            await dbContext.LaunchRecords.FindAsync([request.LaunchRecordId], cancellationToken) is { } record
                ? ModelMapper.Map(record)
                : new NotFound($"Launch record {request.LaunchRecordId} was not found");
    }
}
