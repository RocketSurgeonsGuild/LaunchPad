using System.Runtime.CompilerServices;

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
public static partial class DeleteLaunchRecord
{
    /// <summary>
    ///     The request to delete a launch record
    /// </summary>
    /// <param name="Id">The id of the record to delete</param>
    [Validate]
    public record Request([property: Must(nameof(Request.IsAssigned))] LaunchRecordId Id) : IRequest<Result>
    {
        internal bool IsAssigned(LaunchRecordId id) => id != default;
    }

    /// <summary>What the operation answers: nothing, not found, or forbidden.</summary>
    [Union]
    public readonly partial struct Result
    {
        public partial Result(Unit value);
        public partial Result(NotFound value);
        public partial Result(Forbidden value);
    }

    [UsedImplicitly]
    internal class DeleteLaunchRecordHandler(RocketDbContext dbContext) : IRequestHandler<Request, Result>
    {
        public async ValueTask<Result> Handle(Request request, CancellationToken cancellationToken)
        {
            if (await dbContext.LaunchRecords.FindAsync([request.Id], cancellationToken) is not { } rocket)
            {
                return new NotFound($"Launch record {request.Id} was not found");
            }

            // contrived for testing
            if (rocket.Id == new LaunchRecordId(new("bad361de-a6d5-425a-9cf6-f9b2dd236be6")))
                return new Forbidden("Unable to operate on given record");

            dbContext.Remove(rocket);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Unit.Value;
        }
    }
}
