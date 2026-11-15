using System.Runtime.CompilerServices;

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
    public record Request : IRequest<Result>
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
        public string SerialNumber { get; set; } = null!; // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The type of the rocket
        /// </summary>
        [IsInEnum]
        public RocketType Type { get; set; } // TODO: Make generator that can be used to create a writable view model

        internal bool IsAssigned(RocketId id) => id != default;
    }

    /// <summary>
    ///     The changes to apply to a rocket. They are applied to the rocket's current values, and the resulting <see cref="Request" /> is validated and
    ///     sent through the mediator.
    /// </summary>
    public partial record PatchRequest : IRequest<Result>, IPropertyTracking<Request>
    {
        /// <summary>
        ///     The rocket id
        /// </summary>
        public RocketId Id { get; init; }
    }

    /// <summary>What the operation answers: the updated rocket, or not found.</summary>
    [Union]
    public readonly partial struct Result
    {
        public partial Result(RocketModel value);
        public partial Result(NotFound value);
    }

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
            return  await FindRocket(request.Id, cancellationToken) is not { } rocket 
                ? (Result)new NotFound($"Rocket {request.Id} was not found")  
                :  await mediator.Send(request.ApplyChanges(MapRequest(rocket)), cancellationToken).ConfigureAwait(false);
        }

        private async Task<ReadyRocket?> FindRocket(RocketId id, CancellationToken cancellationToken) =>
            await dbContext.Rockets.FindAsync([id], cancellationToken).ConfigureAwait(false);
    }

    [MapperRequiredMapping(RequiredMappingStrategy.Source)]
    private static partial void Map(Request request, ReadyRocket record);
}
