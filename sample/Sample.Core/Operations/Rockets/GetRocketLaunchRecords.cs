using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
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
public static partial class GetRocketLaunchRecords
{
    [Validate]
    [RequirePolicy(Policies.LaunchRecordsRead)]
    public record Request : IStreamRequest<Result>
    {
        /// <summary>
        ///     The rocket id
        /// </summary>
        [Must(nameof(IsAssigned))]
        public RocketId Id { get; init; }

        internal bool IsAssigned(RocketId id) => !id.IsDefault && id.Value != Guid.Empty;
    }

    /// <summary>
    ///     One item of the stream: a launch record, or not found. A stream of unions ends with its first error case, so a missing rocket answers
    ///     <see cref="NotFound" /> before any launch record.
    /// </summary>
    public readonly union Result(LaunchRecordModel, NotFound, Unauthorized, Forbidden);

    internal class GetRocketLaunchRecordsHandler(RocketDbContext dbContext) : IStreamRequestHandler<Request, Result>
    {
        public async IAsyncEnumerable<Result> Handle(Request request, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (await dbContext.Rockets.FindAsync([request.Id], cancellationToken) is not { } rocket)
            {
                yield return new NotFound($"Rocket {request.Id} was not found");
                yield break;
            }

            var query = ModelMapper.ProjectTo(dbContext.LaunchRecords.Where(z => z.RocketId == rocket.Id));
            await foreach (var item in query.AsAsyncEnumerable().WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }
    }
}
