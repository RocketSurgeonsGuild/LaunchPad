using System.Runtime.CompilerServices;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Riok.Mapperly.Abstractions;

using Rocket.Surgery.LaunchPad.Mapping;
using Rocket.Surgery.LaunchPad.Primitives;

using Sample.Core.Domain;
using Sample.Core.Models;

namespace Sample.Core.Operations.Rockets;

[PublicAPI]
[Mapper]
[UseStaticMapper(typeof(NodaTimeMapper))]
public static partial class GetRocketLaunchRecords
{
    public record Request : IStreamRequest<LaunchRecordModel>
    {
        /// <summary>
        ///     The rocket id
        /// </summary>
        public RocketId Id { get; init; }
    }

    private class Validator : AbstractValidator<Request>
    {
        public Validator() => RuleFor(x => x.Id)
            .NotEmpty()
            .NotNull();
    }

    private class Handler(RocketDbContext dbContext) : IStreamRequestHandler<Request, LaunchRecordModel>
    {
        public async IAsyncEnumerable<LaunchRecordModel> Handle(Request request, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var rocket =  await dbContext.Rockets.FindAsync([request.Id], cancellationToken)  ?? throw new NotFoundException();
            var query = ModelMapper.ProjectTo(dbContext.LaunchRecords.Where(z => z.RocketId == rocket.Id));
            await foreach (var item in query.AsAsyncEnumerable().WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }
    }
}
