
using System.Text.Json.Serialization;
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
public static partial class GetRocket
{
    /// <summary>
    ///     Request to fetch information about a rocket
    /// </summary>
    /// <param name="Id">The id of the rocket</param>
    [Validate]
    [RequirePolicy(Policies.RocketsRead)]
    public readonly record struct Request([property: JsonRequired, Must(nameof(Request.IsAssigned))] RocketId Id) : IRequest<Result>
    {
        internal bool IsAssigned(RocketId id) => !id.IsDefault && id.Value != Guid.Empty;
    }

    /// <summary>What the operation answers: the rocket, or not found; unauthorized or forbidden when the caller may not ask.</summary>
    public readonly union Result(RocketModel, NotFound, Unauthorized, Forbidden);

    internal class GetRocketHandler(RocketDbContext dbContext) : IRequestHandler<Request, Result>
    {
        public async ValueTask<Result> Handle(Request request, CancellationToken cancellationToken) =>
            await dbContext.Rockets.FindAsync([request.Id], cancellationToken).ConfigureAwait(false) is { } rocket
                ? ModelMapper.Map(rocket)
                : new NotFound($"Rocket {request.Id} was not found");
    }
}
