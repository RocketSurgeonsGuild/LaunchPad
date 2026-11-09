using System.Runtime.CompilerServices;

using FluentValidation;

using MediatR;

using Riok.Mapperly.Abstractions;

using Rocket.Surgery.LaunchPad.Primitives;

using Sample.Core.Domain;
using Sample.Core.Models;

namespace Sample.Core.Operations.Rockets;

[PublicAPI]
[Mapper]
[UseStaticMapper(typeof(NodaTimeMapper))]
public static partial class GetRocketLaunchRecord
{
    public record Request : IRequest<Result>
    {
        /// <summary>
        ///     The rocket id
        /// </summary>
        public RocketId Id { get; init; }

        /// <summary>
        ///     The launch record id
        /// </summary>
        public LaunchRecordId LaunchRecordId { get; init; }
    }

    private class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.LaunchRecordId)
               .NotEmpty()
               .NotNull();
        }
    }


    /// <summary>What the operation answers: the launch record, or not found.</summary>
    [Union]
    public readonly partial struct Result
    {
        public partial Result(LaunchRecordModel value);
        public partial Result(NotFound value);
    }

    private class Handler(RocketDbContext dbContext) : IRequestHandler<Request, Result>
    {
        public async Task<Result> Handle(Request request, CancellationToken cancellationToken) =>
            await dbContext.LaunchRecords.FindAsync([request.LaunchRecordId], cancellationToken) is { } record
                ? ModelMapper.Map(record)
                : new NotFound($"Launch record {request.LaunchRecordId} was not found");
    }
}
