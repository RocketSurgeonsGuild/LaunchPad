using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Domain;

namespace Sample.Core.Operations.LaunchRecords;

[PublicAPI]
public static class DeleteLaunchRecord
{
    public record Request : IRequest
    {
        public Guid Id { get; init; }
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

    private class Handler : IRequestHandler<Request>
    {
        private readonly RocketDbContext _dbContext;
        private readonly IMapper _mapper;

        public Handler(RocketDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var rocket = await _dbContext.LaunchRecords.FindAsync(new object[] { request.Id }, cancellationToken);
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
