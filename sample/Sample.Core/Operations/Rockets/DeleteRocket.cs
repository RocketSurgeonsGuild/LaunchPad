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
public static partial class DeleteRocket
{
    /// <summary>
    ///     The request to remove a rocket from the system
    /// </summary>
    /// <param name="Id">The id of the rocket to remove</param>
    [Validate]
    public record Request([property: Must(nameof(Request.IsAssigned))] RocketId Id) : IRequest
    {
        internal bool IsAssigned(RocketId id) => id != default;
    }

    internal class DeleteRocketHandler(RocketDbContext dbContext) : IRequestHandler<Request, Unit>
    {
        public async ValueTask<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var rocket = await dbContext.Rockets.FindAsync([request.Id], cancellationToken) ?? throw new NotFoundException();
            dbContext.Remove(rocket);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Unit.Value;
        }
    }
}
