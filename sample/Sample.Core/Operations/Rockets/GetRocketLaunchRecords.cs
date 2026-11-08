using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;

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
public static partial class GetRocketLaunchRecords
{
    [Validate]
    public record Request : IStreamRequest<LaunchRecordModel>
    {
        /// <summary>
        ///     The rocket id
        /// </summary>
        [Must(nameof(IsAssigned))]
        public RocketId Id { get; init; }

        internal bool IsAssigned(RocketId id) => id != default;
    }

    internal class GetRocketLaunchRecordsHandler(RocketDbContext dbContext) : IStreamRequestHandler<Request, LaunchRecordModel>
    {
        public async IAsyncEnumerable<LaunchRecordModel> Handle(Request request, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var rocket = await dbContext.Rockets.FindAsync([request.Id], cancellationToken) ?? throw new NotFoundException();
            var query = ModelMapper.ProjectTo(dbContext.LaunchRecords.Where(z => z.RocketId == rocket.Id));
            await foreach (var item in query.AsAsyncEnumerable().WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }
    }
}
