using System.Runtime.CompilerServices;

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
    public record Request([property: Must(nameof(Request.IsAssigned))] RocketId Id) : IRequest<Result>
    {
        internal bool IsAssigned(RocketId id) => id != default;
    }

    /// <summary>What the operation answers: the rocket, or not found.</summary>
    [Union]
    public readonly partial struct Result
    {
        public partial Result(RocketModel value);
        public partial Result(NotFound value);
    }

    internal class GetRocketHandler(RocketDbContext dbContext) : IRequestHandler<Request, Result>
    {
        public async ValueTask<Result> Handle(Request request, CancellationToken cancellationToken) =>
            await dbContext.Rockets.FindAsync([request.Id], cancellationToken).ConfigureAwait(false) is { } rocket
                ? ModelMapper.Map(rocket)
                : new NotFound($"Rocket {request.Id} was not found");
    }
}
