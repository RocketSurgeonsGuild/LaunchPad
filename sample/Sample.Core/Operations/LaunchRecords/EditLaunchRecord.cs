using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;

using NodaTime;

using Riok.Mapperly.Abstractions;

using Rocket.Surgery.LaunchPad.Foundation;

using Sample.Core.Domain;
using Sample.Core.Models;

using ZeroAlloc.Mediator;
using ZeroAlloc.Validation;

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
    [Validate]
    public partial record Request : IRequest<Result>
    {
        /// <summary>
        ///     The launch record to update
        /// </summary>
        [Must(nameof(IsAssigned))]
        public LaunchRecordId Id { get; init; }

        /// <summary>
        ///     The updated launch partner
        /// </summary>
        [NotEmpty]
        public string Partner { get; init; } = null!; // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The updated launch payload
        /// </summary>
        [NotEmpty]
        public string Payload { get; init; } = null!; // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The updated payload weight
        /// </summary>
        [GreaterThanOrEqualTo(0d)]
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
        [Must(nameof(IsAssigned))]
        public RocketId RocketId { get; init; } // TODO: Make generator that can be used to create a writable view model

        internal bool IsAssigned(LaunchRecordId id) => id != default;

        internal bool IsAssigned(RocketId id) => id != default;
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

    // The patch's handle method is generated: it applies the patch to the request GetRequest loads and sends the result through the mediator.
    internal partial class EditLaunchRecordHandler(RocketDbContext dbContext, IMediator mediator) : IRequestHandler<Request, Result>, IRequestHandler<PatchRequest, Result>
    {
        public async ValueTask<Result> Handle(Request request, CancellationToken cancellationToken)
        {
            if (await FindLaunchRecord(request.Id, cancellationToken) is not { } rocket) return new NotFound($"Launch record {request.Id} was not found");

            Map(request, rocket);
            dbContext.Update(rocket);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return ModelMapper.Map(rocket);
        }

        private async partial ValueTask<Request> GetRequest(PatchRequest request, CancellationToken cancellationToken) =>
            Map(await GetLaunchRecord(request.Id, cancellationToken).ConfigureAwait(false));

        // The generated patch handle method sends what GetRequest loads, so a patch of a missing launch record still throws NotFoundException here.
        private async Task<LaunchRecord> GetLaunchRecord(LaunchRecordId id, CancellationToken cancellationToken) =>
            await FindLaunchRecord(id, cancellationToken) ?? throw new NotFoundException();

        private async Task<LaunchRecord?> FindLaunchRecord(LaunchRecordId id, CancellationToken cancellationToken) =>
            await dbContext.LaunchRecords.Include(z => z.Rocket).FirstOrDefaultAsync(z => z.Id == id, cancellationToken).ConfigureAwait(false);
    }

    [MapperIgnoreSource(nameof(LaunchRecord.Rocket))]
    private static partial Request Map(LaunchRecord model);

    [MapperIgnoreTarget(nameof(LaunchRecord.Rocket))]
    private static partial void Map(Request request, LaunchRecord record);
}
