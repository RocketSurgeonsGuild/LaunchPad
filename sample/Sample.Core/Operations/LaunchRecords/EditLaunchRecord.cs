using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;

namespace Sample.Core.Operations.LaunchRecords;

[PublicAPI]
public static partial class EditLaunchRecord
{
    /// <summary>
    ///     The launch record update request
    /// </summary>
    public partial record Request : IRequest<LaunchRecordModel>
    {
        /// <summary>
        ///     The launch record to update
        /// </summary>
        public LaunchRecordId Id { get; init; }

        /// <summary>
        ///     The updated launch partner
        /// </summary>
        public string Partner { get; set; } = null!; // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The updated launch payload
        /// </summary>
        public string Payload { get; set; } = null!; // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The updated payload weight
        /// </summary>
        public double PayloadWeightKg { get; set; } // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The updated actual launch date
        /// </summary>
        public Instant? ActualLaunchDate { get; set; } // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The scheduled launch date
        /// </summary>
        public Instant ScheduledLaunchDate { get; set; } // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The update rocket id
        /// </summary>
        public RocketId RocketId { get; set; } // TODO: Make generator that can be used to create a writable view model
    }

    public partial record PatchRequest : IRequest<LaunchRecordModel>, IPropertyTracking<Request>
    {
        /// <summary>
        ///     The rocket id
        /// </summary>
        public LaunchRecordId Id { get; init; }
    }

    private class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Request, LaunchRecord>()
               .ForMember(x => x.Rocket, x => x.Ignore())
               .ForMember(x => x.Id, x => x.Ignore())
                ;
        }
    }

    private class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(z => z.Id)
               .NotEmpty()
               .NotNull();

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

    private class Handler : PatchRequestHandler<Request, PatchRequest, LaunchRecordModel>, IRequestHandler<Request, LaunchRecordModel>
    {
        private readonly RocketDbContext _dbContext;
        private readonly IMapper _mapper;

        public Handler(RocketDbContext dbContext, IMapper mapper, IMediator mediator) : base(mediator)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        private async Task<LaunchRecord> GetLaunchRecord(LaunchRecordId id, CancellationToken cancellationToken)
        {
            var rocket = await _dbContext.LaunchRecords
                                         .Include(z => z.Rocket)
                                         .FirstOrDefaultAsync(z => z.Id == id, cancellationToken)
                                         .ConfigureAwait(false);
            if (rocket == null)
            {
                throw new NotFoundException();
            }

            return rocket;
        }

        protected override async Task<Request> GetRequest(PatchRequest patchRequest, CancellationToken cancellationToken)
        {
            var rocket = await GetLaunchRecord(patchRequest.Id, cancellationToken);
            return _mapper.Map<Request>(_mapper.Map<LaunchRecordModel>(rocket));
        }

        public async Task<LaunchRecordModel> Handle(Request request, CancellationToken cancellationToken)
        {
            var rocket = await GetLaunchRecord(request.Id, cancellationToken);

            _mapper.Map(request, rocket);
            _dbContext.Update(rocket);
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return _mapper.Map<LaunchRecordModel>(rocket);
        }
    }
}
