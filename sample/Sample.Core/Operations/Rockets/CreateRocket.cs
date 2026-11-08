using Microsoft.EntityFrameworkCore;

using Riok.Mapperly.Abstractions;

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
public static partial class CreateRocket
{
    /// <summary>
    ///     The operation to create a new rocket record
    /// </summary>
    [Validate]
    public record Request : IRequest<Response>
    {
        /// <summary>
        ///     The serial number of the rocket
        /// </summary>
        [NotEmpty]
        [MinLength(10)]
        [MaxLength(30)]
        public string SerialNumber { get; set; } = null!; // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The type of rocket
        /// </summary>
        [IsInEnum]
        public RocketType Type { get; set; } // TODO: Make generator that can be used to create a writable view model
    }

    /// <summary>
    ///     The identifier of the rocket that was created
    /// </summary>
    public record Response
    {
        /// <summary>
        ///     The rocket id
        /// </summary>
        public RocketId Id { get; init; }
    }

    internal class CreateRocketHandler(RocketDbContext dbContext) : IRequestHandler<Request, Response>
    {
        public async ValueTask<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var existingRocket = await dbContext
                                      .Rockets.AsQueryable()
                                      .FirstOrDefaultAsync(z => z.SerialNumber == request.SerialNumber, cancellationToken);
            if (existingRocket is { })
            {
                throw new RequestFailedException("A Rocket already exists with that serial number!")
                {
                    Title = "Rocket Creation Failed",
                    Properties = new Dictionary<string, object?>
                    {
                        ["data"] = new
                        {
                            id = existingRocket.Id,
                            type = existingRocket.Type,
                            sn = existingRocket.SerialNumber,
                        },
                    },
                };
            }

            var rocket = Map(request);
            await dbContext.AddAsync(rocket, cancellationToken).ConfigureAwait(false);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new()
            {
                Id = rocket.Id,
            };
        }
    }

    [MapperRequiredMapping(RequiredMappingStrategy.Source)]
    private static partial ReadyRocket Map(Request request);
}
