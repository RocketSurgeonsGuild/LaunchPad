using Microsoft.EntityFrameworkCore;

using Riok.Mapperly.Abstractions;

using Rocket.Surgery.LaunchPad.Primitives;

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
    public record Request([property: Must(nameof(Request.IsAssigned))] LaunchRecordId Id) : IRequest<LaunchRecordModel>
    {
        internal bool IsAssigned(LaunchRecordId id) => id != default;
    }

    internal class GetLaunchRecordHandler(RocketDbContext dbContext) : IRequestHandler<Request, LaunchRecordModel>
    {
        public async ValueTask<LaunchRecordModel> Handle(Request request, CancellationToken cancellationToken)
        {
            var rocket = await dbContext
                              .LaunchRecords
                              .Include(x => x.Rocket)
                              .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            return rocket is null ? throw new NotFoundException() : ModelMapper.Map(rocket);
        }
    }
}
