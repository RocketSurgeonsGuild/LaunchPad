using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using NodaTime;
using Rocket.Surgery.LaunchPad.Extensions;
using Sample.Core.Domain;

namespace Sample.Core.Operations.LaunchRecords
{
    [PublicAPI]
    public static class CreateLaunchRecord
    {
        public class Request : IRequest<Response>
        {
            public string Partner { get; set; } = null!;
            public string Payload { get; set; } = null!;
            public double PayloadWeightKg { get; set; }
            public Instant? ActualLaunchDate { get; set; }
            public Instant ScheduledLaunchDate { get; set; }
            public Guid RocketId { get; set; }
        }

        public class Response
        {
            public Guid Id { get; set; }
        }

        class Mapper : Profile
        {
            public Mapper()
            {
                CreateMap<Request, LaunchRecord>()
                   .ForMember(x => x.RocketId, x => x.Ignore())
                   .ForMember(x => x.Rocket, x => x.Ignore())
                   .ForMember(x => x.Id, x => x.Ignore())
                    ;
            }
        }

        class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Partner)
                   .NotEmpty()
                   .NotNull();
                RuleFor(x => x.RocketId)
                   .NotEmpty()
                   .NotNull();
                RuleFor(x => x.Payload)
                   .NotEmpty()
                   .NotNull();
                RuleFor(x => x.ActualLaunchDate);
                RuleFor(x => x.ScheduledLaunchDate)
                   .NotNull();
                RuleFor(x => x.PayloadWeightKg)
                   .GreaterThanOrEqualTo(0d);

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
                var record = _mapper.Map<LaunchRecord>(request);

                var rocket = await _dbContext.Rockets.FindAsync(new object[] { request.RocketId }, cancellationToken);
                if (rocket == null)
                {
                    throw new RequestFailedException("Rocket not found!");
                }

                record.Rocket = rocket;

                await _dbContext.AddAsync(record, cancellationToken).ConfigureAwait(false);
                await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return new Response()
                {
                    Id = record.Id
                };
            }
        }
    }
}