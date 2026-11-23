using System.ComponentModel.DataAnnotations;
using NodaTime;
using Riok.Mapperly.Abstractions;
using Rocket.Surgery.LaunchPad.Api;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Domain;
using Sample.Core.Models;
using ZeroAlloc.Mediator;
using ZeroAlloc.Validation;

namespace Sample.Core.Operations.LaunchRecords;

[PublicAPI]
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Source)]
[UseStaticMapper(typeof(NodaTimeMapper))]
[UseStaticMapper(typeof(ModelMapper))]
[UseStaticMapper(typeof(StandardMapper))]
public static partial class CreateLaunchRecord
{
    /// <summary>
    ///     Create a launch record
    /// </summary>
    [Validate]
    public readonly record struct Request : IRequest<Result>
    {
        /// <summary>
        ///     The rocket to use
        /// </summary>
        // Required for the OpenAPI document; the ZeroAlloc rule checks that it is assigned.
        [Required]
        [Must(nameof(IsAssigned))]
        public RocketId RocketId { get; init; } // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The launch partner
        /// </summary>
        [NotEmpty]
        public string? Partner { get; init; } // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The launch partners payload
        /// </summary>
        [NotEmpty]
        public string? Payload { get; init; } // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The payload weight
        /// </summary>
        [GreaterThanOrEqualTo(0d)]
        public double PayloadWeightKg { get; init; } // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The actual launch date
        /// </summary>
        public OffsetDateTime? ActualLaunchDate { get; init; } // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The intended launch date
        /// </summary>
        [Required]
        public OffsetDateTime ScheduledLaunchDate { get; init; } // TODO: Make generator that can be used to create a writable view model

        internal bool IsAssigned(RocketId id) => !id.IsDefault && id.Value != Guid.Empty;
    }

    /// <summary>
    ///     The launch record creation response
    /// </summary>
    [Creates(typeof(GetLaunchRecord.Request))]
    public record Response
    {
        /// <summary>
        ///     The id of the new launch record
        /// </summary>
        public LaunchRecordId Id { get; init; }
    }

    /// <summary>What the operation answers: the new launch record's id, or not found when its rocket doesn't exist.</summary>
    public readonly union Result(Response, NotFound);

    internal class CreateLaunchRecordHandler(RocketDbContext dbContext) : IRequestHandler<Request, Result>
    {
        public async ValueTask<Result> Handle(Request request, CancellationToken cancellationToken)
        {
            var record = Map(request);

            if (await dbContext.Rockets.FindAsync([request.RocketId], cancellationToken) is not { } rocket)
            {
                return new NotFound($"Rocket {request.RocketId} was not found");
            }

            record.Rocket = rocket;

            await dbContext.AddAsync(record, cancellationToken).ConfigureAwait(false);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new Response
            {
                Id = record.Id,
            };
        }
    }

    private static partial LaunchRecord Map(Request request);
}
