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
public static partial class GetRocket
{
    /// <summary>
    ///     Request to fetch information about a rocket
    /// </summary>
    /// <param name="Id">The id of the rocket</param>
    [Validate]
    public record Request([property: Must(nameof(Request.IsAssigned))] RocketId Id) : IRequest<RocketModel>
    {
        internal bool IsAssigned(RocketId id) => id != default;
    }

    internal class GetRocketHandler(RocketDbContext dbContext) : IRequestHandler<Request, RocketModel>
    {
        public async ValueTask<RocketModel> Handle(Request request, CancellationToken cancellationToken) =>
            ModelMapper.Map(await dbContext.Rockets.FindAsync([request.Id], cancellationToken).ConfigureAwait(false) ?? throw new NotFoundException());
    }
}
