using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Domain;
using Sample.Core.Models;

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

    private class Handler : IRequestHandler<Request, LaunchRecordModel>
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
                = await _dbContext.LaunchRecords
                                  .Include(z => z.Rocket)
                                  .FirstOrDefaultAsync(z => z.Id == request.Id, cancellationToken)
                                  .ConfigureAwait(false);
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
