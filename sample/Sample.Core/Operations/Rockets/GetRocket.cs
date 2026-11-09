using System.Runtime.CompilerServices;

using FluentValidation;

using MediatR;

using Riok.Mapperly.Abstractions;

using Rocket.Surgery.LaunchPad.Primitives;

using Sample.Core.Domain;
using Sample.Core.Models;

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
    public record Request(RocketId Id) : IRequest<Result>;

    /// <summary>What the operation answers: the rocket, or not found.</summary>
    [Union]
    public readonly partial struct Result
    {
        public partial Result(RocketModel value);
        public partial Result(NotFound value);
    }

    private class Validator : AbstractValidator<Request>
    {
        public Validator() => RuleFor(x => x.Id)
                             .NotEmpty()
                             .NotNull();
    }

    private class Handler(RocketDbContext dbContext) : IRequestHandler<Request, Result>
    {
        public async Task<Result> Handle(Request request, CancellationToken cancellationToken) =>
            await dbContext.Rockets.FindAsync([request.Id], cancellationToken).ConfigureAwait(false) is { } rocket
                ? ModelMapper.Map(rocket)
                : new NotFound($"Rocket {request.Id} was not found");
    }
}
