using AutoMapper;
using FluentValidation;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Domain;
using Sample.Core.Models;

namespace Sample.Core.Operations.Rockets;

[PublicAPI]
public static class GetRocket
{
    /// <summary>
    ///     Request to fetch information about a rocket
    /// </summary>
    public record Request : IRequest<RocketModel>
    {
        /// <summary>
        ///     The rocket id
        /// </summary>
        public RocketId Id { get; set; }
    }

    private class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
               .NotEmpty()
               .NotNull();
        }
    }

    private class Handler(RocketDbContext dbContext, IMapper mapper) : IRequestHandler<Request, RocketModel>
    {
        public async Task<RocketModel> Handle(Request request, CancellationToken cancellationToken)
        {
            var rocket = await dbContext.Rockets.FindAsync(new object[] { request.Id, }, cancellationToken).ConfigureAwait(false);
            if (rocket == null) throw new NotFoundException();

            return mapper.Map<RocketModel>(rocket);
        }
    }
}