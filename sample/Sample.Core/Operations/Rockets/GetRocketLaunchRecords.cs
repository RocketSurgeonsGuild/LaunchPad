using System.Runtime.CompilerServices;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Domain;
using Sample.Core.Models;

namespace Sample.Core.Operations.Rockets;

[PublicAPI]
public static class GetRocketLaunchRecords
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
        public Validator()
        {
            RuleFor(x => x.Id)
               .NotEmpty()
               .NotNull();
        }
    }

    private class Handler(RocketDbContext dbContext, IMapper mapper) : IStreamRequestHandler<Request, LaunchRecordModel>
    {
        public async IAsyncEnumerable<LaunchRecordModel> Handle(Request request, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var rocket = await dbContext.Rockets.FindAsync(new object[] { request.Id }, cancellationToken);
            if (rocket == null) throw new NotFoundException();

            var query = dbContext.LaunchRecords.AsQueryable()
                                 .Where(z => z.RocketId == rocket.Id)
                                 .ProjectTo<LaunchRecordModel>(mapper.ConfigurationProvider);
            await foreach (var item in query.AsAsyncEnumerable().WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }
    }
}
