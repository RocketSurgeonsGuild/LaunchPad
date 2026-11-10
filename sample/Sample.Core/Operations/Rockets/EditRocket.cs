using Riok.Mapperly.Abstractions;

using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.Primitives;

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
    public record Request : IRequest<RocketModel>
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
    public partial record PatchRequest : IRequest<RocketModel>, IPropertyTracking<Request>
    {
        /// <summary>
        ///     The rocket id
        /// </summary>
        public RocketId Id { get; init; }
    }

    // The patch's handle method is generated: it applies the patch to the request GetRequest loads and sends the result through the mediator.
    internal partial class EditRocketHandler(RocketDbContext dbContext, IMediator mediator) : IRequestHandler<Request, RocketModel>, IRequestHandler<PatchRequest, RocketModel>
    {
        public async ValueTask<RocketModel> Handle(Request request, CancellationToken cancellationToken)
        {
            var rocket = await GetRocket(request.Id, cancellationToken) ?? throw new NotFoundException();
            Map(request, rocket);
            dbContext.Update(rocket);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return ModelMapper.Map(rocket);
        }

        private async Task<ReadyRocket> GetRocket(RocketId id, CancellationToken cancellationToken) => await dbContext
                                                                                                            .Rockets.FindAsync([id], cancellationToken)
                                                                                                            .ConfigureAwait(false)
         ?? throw new NotFoundException();
    }

    [MapperRequiredMapping(RequiredMappingStrategy.Source)]
    private static partial void Map(Request request, ReadyRocket record);
}
