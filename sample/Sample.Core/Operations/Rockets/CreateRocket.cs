using System.Runtime.CompilerServices;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Riok.Mapperly.Abstractions;

using Rocket.Surgery.LaunchPad.Api;
using Rocket.Surgery.LaunchPad.Primitives;

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
    /// <summary>
    ///     The operation to create a new rocket record
    /// </summary>
    public record Request : IRequest<Result>
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
    [Creates(typeof(GetRocket.Request))]
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


    /// <summary>What the operation answers: the new rocket's id, or a conflict with an existing rocket.</summary>
    [Union]
    public readonly partial struct Result
    {
        public partial Result(Response value);
        public partial Result(Conflict value);
    }

    private class Handler(RocketDbContext dbContext) : IRequestHandler<Request, Result>
    {
        public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
        {
            var existingRocket = await dbContext
                                      .Rockets.AsQueryable()
                                      .FirstOrDefaultAsync(z => z.SerialNumber == request.SerialNumber, cancellationToken);
            if (existingRocket is { })
            {
                return new Conflict("A Rocket already exists with that serial number!")
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

            return new Response
            {
                Id = rocket.Id,
            };
        }
    }

    [MapperRequiredMapping(RequiredMappingStrategy.Source)]
    private static partial ReadyRocket Map(Request request);
}
