using System.Runtime.CompilerServices;

using FluentValidation;

using MediatR;

using NodaTime;

using Riok.Mapperly.Abstractions;

using Rocket.Surgery.LaunchPad.Api;
using Rocket.Surgery.LaunchPad.Primitives;

using Sample.Core.Domain;
using Sample.Core.Models;

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
    public record Request : IRequest<Result>
    {
        /// <summary>
        ///     The rocket to use
        /// </summary>
        public RocketId RocketId { get; set; } // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The launch partner
        /// </summary>
        public string? Partner { get; set; } // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The launch partners payload
        /// </summary>
        public string? Payload { get; set; } // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The payload weight
        /// </summary>
        public double PayloadWeightKg { get; set; } // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The actual launch date
        /// </summary>
        public Instant? ActualLaunchDate { get; set; } // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The intended launch date
        /// </summary>
        public Instant ScheduledLaunchDate { get; set; } // TODO: Make generator that can be used to create a writable view model
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

    private class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
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

    /// <summary>What the operation answers: the new launch record's id, or not found when its rocket doesn't exist.</summary>
    [Union]
    public readonly partial struct Result
    {
        public partial Result(Response value);
        public partial Result(NotFound value);
    }

    private class Handler(RocketDbContext dbContext) : IRequestHandler<Request, Result>
    {
        public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
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
