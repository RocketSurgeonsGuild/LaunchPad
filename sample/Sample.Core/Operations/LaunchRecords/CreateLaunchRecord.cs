using System.ComponentModel.DataAnnotations;

using NodaTime;

using Riok.Mapperly.Abstractions;
using Rocket.Surgery.LaunchPad.Primitives;

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
    public record Request : IRequest<Response>
    {
        /// <summary>
        ///     The rocket to use
        /// </summary>
        // Required for the OpenAPI document; the ZeroAlloc rule checks that it is assigned.
        [Required]
        [Must(nameof(IsAssigned))]
        public RocketId RocketId { get; set; } // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The launch partner
        /// </summary>
        [NotEmpty]
        public string? Partner { get; set; } // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The launch partners payload
        /// </summary>
        [NotEmpty]
        public string? Payload { get; set; } // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The payload weight
        /// </summary>
        [GreaterThanOrEqualTo(0d)]
        public double PayloadWeightKg { get; set; } // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The actual launch date
        /// </summary>
        public Instant? ActualLaunchDate { get; set; } // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The intended launch date
        /// </summary>
        [Required]
        public Instant ScheduledLaunchDate { get; set; } // TODO: Make generator that can be used to create a writable view model

        internal bool IsAssigned(RocketId id) => id != default;
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

    internal class CreateLaunchRecordHandler(RocketDbContext dbContext) : IRequestHandler<Request, Response>
    {
        public async ValueTask<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var record = Map(request);

            var rocket = await dbContext.Rockets.FindAsync([request.RocketId], cancellationToken) ?? throw new RequestFailedException("Rocket not found!");
            record.Rocket = rocket;

            await dbContext.AddAsync(record, cancellationToken).ConfigureAwait(false);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new()
            {
                Id = record.Id,
            };
        }
    }

    private static partial LaunchRecord Map(Request request);
}
