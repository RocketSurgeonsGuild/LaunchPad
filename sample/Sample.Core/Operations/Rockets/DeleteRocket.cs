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
public static partial class DeleteRocket
{
    /// <summary>
    ///     The request to remove a rocket from the system
    /// </summary>
    /// <param name="Id">The id of the rocket to remove</param>
    public record Request(RocketId Id) : IRequest<Result>;

    /// <summary>What the operation answers: nothing, or not found.</summary>
    [Union]
    public readonly partial struct Result
    {
        public partial Result(Unit value);
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
        public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
        {
            if (await dbContext.Rockets.FindAsync([request.Id], cancellationToken) is not { } rocket) return new NotFound($"Rocket {request.Id} was not found");
            dbContext.Remove(rocket);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Unit.Value;
        }
    }
}
