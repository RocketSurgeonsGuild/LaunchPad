using FluentValidation;

using MediatR;

using Riok.Mapperly.Abstractions;

using Rocket.Surgery.LaunchPad.Mapping;
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
    public record Request(RocketId Id) : IRequest<RocketModel>;

    private class Validator : AbstractValidator<Request>
    {
        public Validator() => RuleFor(x => x.Id)
            .NotEmpty()
            .NotNull();
    }

    private class Handler(RocketDbContext dbContext) : IRequestHandler<Request, RocketModel>
    {
        public async Task<RocketModel> Handle(Request request, CancellationToken cancellationToken) =>
            ModelMapper.Map(await dbContext.Rockets.FindAsync([request.Id], cancellationToken).ConfigureAwait(false) ?? throw new NotFoundException());
    }
}
