using System.Runtime.CompilerServices;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Riok.Mapperly.Abstractions;

using Rocket.Surgery.LaunchPad.Primitives;

using Sample.Core.Domain;
using Sample.Core.Models;

namespace Sample.Core.Operations.LaunchRecords;

[PublicAPI]
[Mapper]
[UseStaticMapper(typeof(NodaTimeMapper))]
public static partial class GetLaunchRecord
{
    /// <summary>
    ///     The request to get a launch record
    /// </summary>
    /// <param name="Id">The id of the launch record</param>
    public record Request(LaunchRecordId Id) : IRequest<Result>;

    /// <summary>What the operation answers: the launch record, or not found.</summary>
    [Union]
    public readonly partial struct Result
    {
        public partial Result(LaunchRecordModel value);
        public partial Result(NotFound value);
    }

    private class Validator : AbstractValidator<Request>
    {
        public Validator() => RuleFor(x => x.Id)
                             .NotEmpty()
                             .NotNull();
    }

    private class Handler(RocketDbContext dbContext) : IRequestHandler<Request, Result>
    {
        public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
        {
            var rocket = await dbContext
                              .LaunchRecords
                              .Include(x => x.Rocket)
                              .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            return rocket is null ? new NotFound($"Launch record {request.Id} was not found") : ModelMapper.Map(rocket);
        }
    }
}
