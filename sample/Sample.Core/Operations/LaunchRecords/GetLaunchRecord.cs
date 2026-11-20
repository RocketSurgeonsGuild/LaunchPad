
using Microsoft.EntityFrameworkCore;

using Riok.Mapperly.Abstractions;

using Rocket.Surgery.LaunchPad.Foundation;

using Sample.Core.Domain;
using Sample.Core.Models;

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
    public readonly record struct Request([property: Must(nameof(Request.IsAssigned))] LaunchRecordId Id) : IRequest<Result>
    {
        internal bool IsAssigned(LaunchRecordId id) => id != default;
    }

    /// <summary>What the operation answers: the launch record, or not found.</summary>
    public readonly union Result(LaunchRecordModel, NotFound);

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
