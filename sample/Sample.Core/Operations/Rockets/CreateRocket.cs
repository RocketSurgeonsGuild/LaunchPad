using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Riok.Mapperly.Abstractions;
using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.Mapping.Profiles;
using Sample.Core.Domain;
using Sample.Core.Models;

namespace Sample.Core.Operations.Rockets;

[PublicAPI]
[Mapper]
[UseStaticMapper(typeof(NodaTimeMapper))]
[UseStaticMapper(typeof(ModelMapper))]
[UseStaticMapper(typeof(StandardMapper))]
public static partial class CreateRocket
{
    [MapperRequiredMapping(RequiredMappingStrategy.Source)]
    private static partial ReadyRocket Map(Request request);

    /// <summary>
    ///     The operation to create a new rocket record
    /// </summary>
    public record Request : IRequest<Response>
    {
        /// <summary>
        ///     The serial number of the rocket
        /// </summary>
        public string SerialNumber { get; set; } = null!; // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The type of rocket
        /// </summary>
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

    private class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Type)
               .IsInEnum();

            RuleFor(x => x.SerialNumber)
               .NotEmpty()
               .MinimumLength(10)
               .MaximumLength(30);
        }
    }

    private class Handler(RocketDbContext dbContext) : IRequestHandler<Request, Response>
    {
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var existingRocket = await dbContext
                                      .Rockets.AsQueryable()
                                      .FirstOrDefaultAsync(z => z.SerialNumber == request.SerialNumber, cancellationToken);
            if (existingRocket != null)
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

            var rocket = Map(request);
            await dbContext.AddAsync(rocket, cancellationToken).ConfigureAwait(false);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new()
            {
                Id = rocket.Id,
            };
        }
    }
}