
using Riok.Mapperly.Abstractions;

using Rocket.Surgery.LaunchPad.Foundation;

using Sample.Core.Domain;
using Sample.Core.Models;

using ZeroAlloc.Mediator;
using ZeroAlloc.Validation;

namespace Sample.Core.Operations.Rockets;

[PublicAPI]
[Mapper]
[UseStaticMapper(typeof(NodaTimeMapper))]
[UseStaticMapper(typeof(ModelMapper))]
[UseStaticMapper(typeof(StandardMapper))]
public static partial class EditRocket
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial Request MapRequest(ReadyRocket model);

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapProperty(nameof(RocketModel.Sn), nameof(Request.SerialNumber))]
    public static partial Request MapRequest(RocketModel model);

    /// <summary>
    ///     The edit operation to update a rocket
    /// </summary>
    [Validate]
    public readonly record struct Request : IRequest<Result>
    {
        /// <summary>
        ///     The rocket id
        /// </summary>
        [Must(nameof(IsAssigned))]
        public RocketId Id { get; init; }

        /// <summary>
        ///     The serial number of the rocket
        /// </summary>
        [NotEmpty]
        [MinLength(10)]
        [MaxLength(30)]
        public string SerialNumber { get; init; }

        /// <summary>
        ///     The type of the rocket
        /// </summary>
        [IsInEnum]
        public RocketType Type { get; init; }

        internal bool IsAssigned(RocketId id) => id != default;
    }

    /// <summary>
    ///     The <see cref="Request" /> a Blazor form binds to: a readonly request can't be edited a field at a time.
    /// </summary>
    [Mutable<Request>]
    public partial class RequestViewModel;

    /// <summary>
    ///     The changes to apply to a rocket. They are applied to the rocket's current values, and the resulting <see cref="Request" /> is validated and
    ///     sent through the mediator.
    /// </summary>
    public readonly partial record struct PatchRequest : IRequest<Result>, IPropertyTracking<Request>
    {
        /// <summary>
        ///     The rocket id
        /// </summary>
        public RocketId Id { get; init; }
    }

    /// <summary>What the operation answers: the updated rocket, or not found.</summary>
    public readonly union Result(RocketModel, NotFound);

    // The patch is handled here rather than by the generated patch handler, whose GetRequest has to return a request and so cannot answer
    // NotFound: a missing rocket answers NotFound, and otherwise the patch is applied to the rocket's current values and sent through the mediator.
    internal partial class EditRocketHandler(RocketDbContext dbContext, IMediator mediator) : IRequestHandler<Request, Result>, IRequestHandler<PatchRequest, Result>
    {
        public async ValueTask<Result> Handle(Request request, CancellationToken cancellationToken)
        {
            if (await FindRocket(request.Id, cancellationToken) is not { } rocket) return new NotFound($"Rocket {request.Id} was not found");
            Map(request, rocket);
            dbContext.Update(rocket);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return ModelMapper.Map(rocket);
        }

        public async ValueTask<Result> Handle(PatchRequest request, CancellationToken cancellationToken)
        {
            return await FindRocket(request.Id, cancellationToken) is not { } rocket
                ? new NotFound($"Rocket {request.Id} was not found")
                : await mediator.Send(request.ApplyChanges(MapRequest(rocket)), cancellationToken).ConfigureAwait(false);
        }

        private async Task<ReadyRocket?> FindRocket(RocketId id, CancellationToken cancellationToken) =>
            await dbContext.Rockets.FindAsync([id], cancellationToken).ConfigureAwait(false);
    }

    [MapperRequiredMapping(RequiredMappingStrategy.Source)]
    private static partial void Map(Request request, ReadyRocket record);
}
