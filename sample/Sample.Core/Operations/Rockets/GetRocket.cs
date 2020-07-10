using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Rocket.Surgery.LaunchPad.Extensions;
using Sample.Core.Domain;
using Sample.Core.Models;

namespace Sample.Core.Operations.Rockets
{
    [PublicAPI]
    public static class GetRocket
    {
        public class Request : IRequest<RocketModel>
        {
            public Guid Id { get; set; }
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

        class Handler : IRequestHandler<Request, RocketModel>
        {
            private readonly RocketDbContext _dbContext;
            private readonly IMapper _mapper;

            public Handler(RocketDbContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }

            public async Task<RocketModel> Handle(Request request, CancellationToken cancellationToken)
            {
                var rocket = await _dbContext.Rockets.FindAsync(new object[] { request.Id }, cancellationToken).ConfigureAwait(false);
                if (rocket == null)
                {
                    throw new NotFoundException();
                }

                return _mapper.Map<RocketModel>(rocket);
            }
        }
    }
}