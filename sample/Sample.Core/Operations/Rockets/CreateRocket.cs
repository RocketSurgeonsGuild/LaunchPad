using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Rocket.Surgery.LaunchPad.Extensions;
using Sample.Core.Domain;

namespace Sample.Core.Operations.Rockets
{
    [PublicAPI]
    public static class CreateRocket
    {
        public class Request : IRequest<Response>
        {
            public string SerialNumber { get; set; } = null!;
            public RocketType Type { get; set; }
        }

        public class Response
        {
            public Guid Id { get; set; }
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

        class Handler : IRequestHandler<Request, Response>
        {
            private readonly RocketDbContext _dbContext;
            private readonly IMapper _mapper;

            public Handler(RocketDbContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var existingRocket = await _dbContext.Rockets
                   .FirstOrDefaultAsync(z => z.SerialNumber == request.SerialNumber, cancellationToken);
                if (existingRocket != null)
                {
                    throw new RequestFailedException(
                        "A Rocket already exists with that serial number!",
                        title: "Rocket Creation Failed",
                        properties: new Dictionary<string, object>()
                        {
                            ["data"] = new
                            {
                                id = existingRocket.Id,
                                type = existingRocket.Type,
                                sn = existingRocket.SerialNumber
                            }
                        }
                    );
                }

                var rocket = _mapper.Map<ReadyRocket>(request);
                await _dbContext.AddAsync(rocket, cancellationToken).ConfigureAwait(false);
                await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return new Response()
                {
                    Id = rocket.Id
                };
            }
        }
    }
}