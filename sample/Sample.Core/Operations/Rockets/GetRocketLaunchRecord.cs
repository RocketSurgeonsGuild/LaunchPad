using Riok.Mapperly.Abstractions;

using Rocket.Surgery.LaunchPad.Primitives;

using Sample.Core.Domain;
using Sample.Core.Models;

using ZeroAlloc.Mediator;
using ZeroAlloc.Validation;

namespace Sample.Core.Operations.Rockets;

[PublicAPI]
[Mapper]
[UseStaticMapper(typeof(NodaTimeMapper))]
public static partial class GetRocketLaunchRecord
{
    [Validate]
    public record Request : IRequest<LaunchRecordModel>
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

        internal bool IsAssigned(RocketId id) => id != default;

        internal bool IsAssigned(LaunchRecordId id) => id != default;
    }

    internal class GetRocketLaunchRecordHandler(RocketDbContext dbContext) : IRequestHandler<Request, LaunchRecordModel>
    {
        public async ValueTask<LaunchRecordModel> Handle(Request request, CancellationToken cancellationToken) =>
            ModelMapper.Map(await dbContext.LaunchRecords.FindAsync([request.LaunchRecordId], cancellationToken) ?? throw new NotFoundException());
    }
}
