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
public static partial class DeleteLaunchRecord
{
    /// <summary>
    ///     The request to delete a launch record
    /// </summary>
    /// <param name="Id">The id of the record to delete</param>
    [Validate]
    public record Request([property: Must(nameof(Request.IsAssigned))] LaunchRecordId Id) : IRequest
    {
        internal bool IsAssigned(LaunchRecordId id) => id != default;
    }

    [UsedImplicitly]
    internal class DeleteLaunchRecordHandler(RocketDbContext dbContext) : IRequestHandler<Request, Unit>
    {
        public async ValueTask<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var rocket = await dbContext.LaunchRecords.FindAsync([request.Id], cancellationToken) ?? throw new NotFoundException();

            // contrived for testing
            if (rocket.Id == new LaunchRecordId(new("bad361de-a6d5-425a-9cf6-f9b2dd236be6")))
                throw new NotAuthorizedException("Unable to operate on given record");

            dbContext.Remove(rocket);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Unit.Value;
        }
    }
}
