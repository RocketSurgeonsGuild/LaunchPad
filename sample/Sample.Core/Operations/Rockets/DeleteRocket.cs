using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.Core.Operations.Rockets
{
    [PublicAPI]
    public static class DeleteRocket
    {
        public record Request : IRequest
        {
            public Guid Id { get; init; }
        }

        class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Id)
                   .NotEmpty()
                   .NotNull();
            }
        }

        class Handler : IRequestHandler<Request>
        {
            private readonly RocketDbContext _dbContext;

            public Handler(RocketDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var rocket = await _dbContext.Rockets.FindAsync(new object[] { request.Id }, cancellationToken);
                if (rocket == null)
                {
                    throw new NotFoundException();
                }

                _dbContext.Remove(rocket);
                await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return Unit.Value;
            }
        }
    }
}