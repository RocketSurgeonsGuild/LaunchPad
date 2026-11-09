using System.Runtime.CompilerServices;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using NodaTime;

using Riok.Mapperly.Abstractions;

using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.Primitives;

using Sample.Core.Domain;
using Sample.Core.Models;

namespace Sample.Core.Operations.LaunchRecords;

[PublicAPI]
[Mapper]
[UseStaticMapper(typeof(NodaTimeMapper))]
[UseStaticMapper(typeof(ModelMapper))]
[UseStaticMapper(typeof(StandardMapper))]
public static partial class EditLaunchRecord
{
    /// <summary>
    ///     The launch record update request
    /// </summary>
    public partial record Request : IRequest<Result>
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
    public partial record PatchRequest(LaunchRecordId Id) : IRequest<Result>, IPropertyTracking<Request>;

    /// <summary>What the operation answers: the updated launch record, or not found.</summary>
    [Union]
    public readonly partial struct Result
    {
        public partial Result(LaunchRecordModel value);
        public partial Result(NotFound value);
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

    private class PatchRequestValidator : AbstractValidator<PatchRequest>
    {
        public PatchRequestValidator()
        {
            RuleFor(x => x.Id)
               .NotEmpty()
               .NotNull();

            RuleFor(x => x.Partner.Value)
               .NotEmpty()
               .NotNull()
               .When(x => x.Partner.HasValue);

            RuleFor(x => x.RocketId.Value)
               .NotEmpty()
               .NotNull()
               .When(x => x.RocketId.HasValue);

            RuleFor(x => x.Payload.Value)
               .NotEmpty()
               .NotNull()
               .When(x => x.Payload.HasValue);

            RuleFor(x => x.ScheduledLaunchDate.Value)
               .NotNull()
               .When(x => x.ScheduledLaunchDate.HasValue);

            RuleFor(x => x.PayloadWeightKg.Value)
               .GreaterThanOrEqualTo(0d)
               .When(x => x.PayloadWeightKg.HasValue);
        }
    }

    private class Handler(RocketDbContext dbContext, IMediator mediator)
        : PatchRequestHandler<Request, PatchRequest, Result>(mediator), IRequestHandler<Request, Result>
    {
        public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
        {
            if (await FindLaunchRecord(request.Id, cancellationToken) is not { } rocket) return NotFound(request.Id);

            Map(request, rocket);
            dbContext.Update(rocket);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return ModelMapper.Map(rocket);
        }

        // A patch of a launch record that doesn't exist answers not found before it is applied.
        public override async Task<Result> Handle(PatchRequest request, CancellationToken cancellationToken) =>
            await FindLaunchRecord(request.Id, cancellationToken) is null ? NotFound(request.Id) : await base.Handle(request, cancellationToken);

        protected override async Task<Request> GetRequest(PatchRequest patchRequest, CancellationToken cancellationToken) =>
            Map(patchRequest, await FindLaunchRecord(patchRequest.Id, cancellationToken) ?? throw new NotFoundException());

        private static NotFound NotFound(LaunchRecordId id) => new($"Launch record {id} was not found");

        private async Task<LaunchRecord?> FindLaunchRecord(LaunchRecordId id, CancellationToken cancellationToken) =>
            await dbContext.LaunchRecords.Include(z => z.Rocket).FirstOrDefaultAsync(z => z.Id == id, cancellationToken).ConfigureAwait(false);
    }

    [MapperIgnoreSource(nameof(LaunchRecord.Rocket))]
    private static partial Request Map(LaunchRecord model);

    [MapperIgnoreTarget(nameof(LaunchRecord.Rocket))]
    private static partial void Map(Request request, LaunchRecord record);

    private static Request Map(PatchRequest request, LaunchRecord record) => request.ApplyChanges(Map(record));
}
