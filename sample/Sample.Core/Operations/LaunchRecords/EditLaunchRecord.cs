using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using NodaTime;
using Rocket.Surgery.LaunchPad.Extensions;
using Sample.Core.Domain;
using Sample.Core.Models;

namespace Sample.Core.Operations.LaunchRecords
{
    [PublicAPI]
    public static class EditLaunchRecord
    {
        public static Request CreateRequest(Guid id, Model model, IMapper mapper) => mapper.Map(model, new Request(id));

        public class Model
        {
            public string Partner { get; set; } = null!;
            public string Payload { get; set; } = null!;
            public double PayloadWeightKg { get; set; }
            public Instant? ActualLaunchDate { get; set; }
            public Instant ScheduledLaunchDate { get; set; }
            public Guid RocketId { get; set; }
        }

        public class Request : Model, IRequest<LaunchRecordModel>
        {
            public Guid Id { get; }

            public Request(Guid id)
            {
                Id = id;
            }

            private Request() { }
        }

        class Mapper : Profile
        {
            public Mapper()
            {
                CreateMap<Request, LaunchRecord>()
                   .ForMember(x => x.Rocket, x => x.Ignore())
                   .ForMember(x => x.Id, x => x.Ignore())
                    ;
                CreateMap<Model, Request>();
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

        class Handler : IRequestHandler<Request, LaunchRecordModel>
        {
            private readonly RocketDbContext _dbContext;
            private readonly IMapper _mapper;

            public Handler(RocketDbContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }

            public async Task<LaunchRecordModel> Handle(Request request, CancellationToken cancellationToken)
            {
                var rocket
                    = await _dbContext.LaunchRecords.FindAsync(new object[] { request.Id }, cancellationToken).ConfigureAwait(false);
                if (rocket == null)
                {
                    throw new NotFoundException();
                }

                _mapper.Map(request, rocket);
                _dbContext.Update(rocket);
                await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return _mapper.Map<LaunchRecordModel>(rocket);
            }
        }
    }
}