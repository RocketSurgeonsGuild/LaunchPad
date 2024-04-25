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
        public string Partner { get; init; } = null!; // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The updated launch payload
        /// </summary>
        public string Payload { get; init; } = null!; // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The updated payload weight
        /// </summary>
        public double PayloadWeightKg { get; init; } // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The updated actual launch date
        /// </summary>
        public Instant? ActualLaunchDate { get; init; } // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The scheduled launch date
        /// </summary>
        public Instant ScheduledLaunchDate { get; init; } // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The update rocket id
        /// </summary>
        public RocketId RocketId { get; init; } // TODO: Make generator that can be used to create a writable view model
    }

    /// <summary>
    ///     The patch request
    /// </summary>
    /// <param name="Id">The rocket id</param>
    public partial record PatchRequest(LaunchRecordId Id) : IRequest<LaunchRecordModel>, IPropertyTracking<Request>;

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

    private class Handler(RocketDbContext dbContext, IMapper mapper, IMediator mediator)
        : PatchRequestHandler<Request, PatchRequest, LaunchRecordModel>(mediator), IRequestHandler<Request, LaunchRecordModel>
    {
        private async Task<LaunchRecord> GetLaunchRecord(LaunchRecordId id, CancellationToken cancellationToken)
        {
            var rocket = await dbContext
                              .LaunchRecords
                              .Include(z => z.Rocket)
                              .FirstOrDefaultAsync(z => z.Id == id, cancellationToken)
                              .ConfigureAwait(false);
            if (rocket == null) throw new NotFoundException();

            return rocket;
        }

        protected override async Task<Request> GetRequest(PatchRequest patchRequest, CancellationToken cancellationToken)
        {
            var rocket = await GetLaunchRecord(patchRequest.Id, cancellationToken);
            return mapper.Map<Request>(mapper.Map<LaunchRecordModel>(rocket));
        }

        public async Task<LaunchRecordModel> Handle(Request request, CancellationToken cancellationToken)
        {
            var rocket = await GetLaunchRecord(request.Id, cancellationToken);

            mapper.Map(request, rocket);
            dbContext.Update(rocket);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return mapper.Map<LaunchRecordModel>(rocket);
        }
    }
}