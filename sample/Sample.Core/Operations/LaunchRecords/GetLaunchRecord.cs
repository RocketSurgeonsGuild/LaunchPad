
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Riok.Mapperly.Abstractions;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Authorization;
using Sample.Core.Domain;
using Sample.Core.Models;
using ZeroAlloc.Authorization;
using ZeroAlloc.Mediator;
using ZeroAlloc.Validation;

namespace Sample.Core.Operations.LaunchRecords;

[PublicAPI]
[Mapper]
[UseStaticMapper(typeof(NodaTimeMapper))]
public static partial class GetLaunchRecord
{
    /// <summary>
    ///     The request to get a launch record
    /// </summary>
    /// <param name="Id">The id of the launch record</param>
    [Validate]
    [RequirePolicy(Policies.LaunchRecordsRead)]
    public readonly record struct Request([property: JsonRequired, Must(nameof(Request.IsAssigned))] LaunchRecordId Id) : IRequest<Result>
    {
        internal bool IsAssigned(LaunchRecordId id) => !id.IsDefault && id.Value != Guid.Empty;
    }

    /// <summary>What the operation answers: the launch record, or not found; unauthorized or forbidden when the caller may not ask.</summary>
    public readonly union Result(LaunchRecordModel, NotFound, Unauthorized, Forbidden);

    internal class GetLaunchRecordHandler(RocketDbContext dbContext) : IRequestHandler<Request, Result>
    {
        public async ValueTask<Result> Handle(Request request, CancellationToken cancellationToken)
        {
            var rocket = await dbContext
                              .LaunchRecords
                              .Include(x => x.Rocket)
                              .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            return rocket is null ? new NotFound($"Launch record {request.Id} was not found") : ModelMapper.Map(rocket);
        }
    }
}
