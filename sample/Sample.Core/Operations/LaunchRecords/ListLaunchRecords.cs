using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sample.Core.Domain;
using Sample.Core.Models;

namespace Sample.Core.Operations.LaunchRecords;

[PublicAPI]
public static class ListLaunchRecords
{
    /// <summary>
    ///     The launch record search
    /// </summary>
    /// <param name="RocketType">The rocket type</param>
    // TODO: Paging model!
    public record Request(RocketType? RocketType) : IStreamRequest<LaunchRecordModel>;

    private class Validator : AbstractValidator<Request>;

    private class Handler(RocketDbContext dbContext, IMapper mapper) : IStreamRequestHandler<Request, LaunchRecordModel>
    {
        public IAsyncEnumerable<LaunchRecordModel> Handle(Request request, CancellationToken cancellationToken)
        {
            var query = dbContext
                       .LaunchRecords
                       .Include(x => x.Rocket)
                       .AsQueryable();
            if (request.RocketType.HasValue) query = query.Where(z => z.Rocket.Type == request.RocketType);

            return query
                  .ProjectTo<LaunchRecordModel>(mapper.ConfigurationProvider)
                  .ToAsyncEnumerable();
        }
    }
}