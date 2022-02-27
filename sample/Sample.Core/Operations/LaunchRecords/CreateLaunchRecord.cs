using AutoMapper;
using FluentValidation;
using MediatR;
using NodaTime;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Domain;
using Sample.Core.Models;

namespace Sample.Core.Operations.LaunchRecords;

[PublicAPI]
public static class CreateLaunchRecord
{
    /// <summary>
    ///     Create a launch record
    /// </summary>
    public record Request : IRequest<Response>
    {
        /// <summary>
        ///     The rocket to use
        /// </summary>
        public RocketId RocketId { get; set; } // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The launch partner
        /// </summary>
        public string? Partner { get; set; } = null!; // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The launch partners payload
        /// </summary>
        public string? Payload { get; set; } = null!; // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The payload weight
        /// </summary>
        public double PayloadWeightKg { get; set; } // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The actual launch date
        /// </summary>
        public Instant? ActualLaunchDate { get; set; } // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The intended launch date
        /// </summary>
        public Instant ScheduledLaunchDate { get; set; } // TODO: Make generator that can be used to create a writable view model
    }

    /// <summary>
    ///     The launch record creation response
    /// </summary>
    public record Response
    {
        /// <summary>
        ///     The id of the new launch record
        /// </summary>
        public LaunchRecordId Id { get; init; }
    }

    private class Mapper : Profile
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

    private class Validator : AbstractValidator<Request>
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

    private class Handler : IRequestHandler<Request, Response>
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

            return new Response
            {
                Id = record.Id
            };
        }
    }
}
