
using System.Text.Json.Serialization;

using Riok.Mapperly.Abstractions;

using Rocket.Surgery.LaunchPad.Foundation;

using Sample.Core.Domain;
using Sample.Core.Models;

using ZeroAlloc.Mediator;
using ZeroAlloc.Validation;

namespace Sample.Core.Operations.Rockets;

[PublicAPI]
[Mapper]
[UseStaticMapper(typeof(NodaTimeMapper))]
public static partial class DeleteRocket
{
    /// <summary>
    ///     The request to remove a rocket from the system
    /// </summary>
    /// <param name="Id">The id of the rocket to remove</param>
    [Validate]
    public readonly record struct Request([property: JsonRequired, Must(nameof(Request.IsAssigned))] RocketId Id) : IRequest<Result>
    {
        internal bool IsAssigned(RocketId id) => id != default;
    }

    /// <summary>What the operation answers: nothing, or not found.</summary>
    public readonly union Result(Unit, NotFound);

    internal class DeleteRocketHandler(RocketDbContext dbContext) : IRequestHandler<Request, Result>
    {
        public async ValueTask<Result> Handle(Request request, CancellationToken cancellationToken)
        {
            if (await dbContext.Rockets.FindAsync([request.Id], cancellationToken) is not { } rocket) return new NotFound($"Rocket {request.Id} was not found");
            dbContext.Remove(rocket);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Unit.Value;
        }
    }
}
