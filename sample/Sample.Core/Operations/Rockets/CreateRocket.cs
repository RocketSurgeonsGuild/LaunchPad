using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Rocket.Surgery.LaunchPad.Extensions;
using Sample.Core.Domain;

namespace Sample.Core.Operations.Rockets
{
    [PublicAPI]
    public static class CreateRocket
    {
        public class Request : IRequest<Guid>
        {
            public string SerialNumber { get; set; } = null!;
            public RocketType Type { get; set; }
        }

        class Mapper : Profile
        {
            public Mapper()
            {
                CreateMap<Request, ReadyRocket>();
            }
        }

        class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Type)
                   .IsInEnum();

                RuleFor(x => x.SerialNumber)
                   .NotEmpty()
                   .MinimumLength(10)
                   .MaximumLength(30);
            }
        }

        class Handler : IRequestHandler<Request, Guid>
        {
            private readonly RocketDbContext _dbContext;
            private readonly IMapper _mapper;

            public Handler(RocketDbContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }

            public async Task<Guid> Handle(Request request, CancellationToken cancellationToken)
            {
                if (_dbContext.Rockets.Any(z => z.SerialNumber == request.SerialNumber))
                {
                    throw new RequestException("A Rocket already exists with that serialnumber!");
                }

                var rocket = _mapper.Map<ReadyRocket>(request);
                await _dbContext.AddAsync(rocket, cancellationToken).ConfigureAwait(false);
                await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return rocket.Id;
            }
        }
    }
}