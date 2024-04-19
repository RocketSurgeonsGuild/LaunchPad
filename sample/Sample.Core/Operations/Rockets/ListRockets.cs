using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sample.Core.Domain;
using Sample.Core.Models;

namespace Sample.Core.Operations.Rockets;

[PublicAPI]
public static class ListRockets
{
    // TODO: Paging model!
    /// <summary>
    ///     The request to search for different rockets
    /// </summary>
    /// <param name="RocketType">The type of the rocket</param>
    public record Request(RocketType? RocketType) : IStreamRequest<RocketModel>;

    private class Validator : AbstractValidator<Request>;

    private class Handler(RocketDbContext dbContext, IMapper mapper) : IStreamRequestHandler<Request, RocketModel>
    {
        public IAsyncEnumerable<RocketModel> Handle(Request request, CancellationToken cancellationToken)
        {
            var query = dbContext.Rockets.AsQueryable();
            if (request.RocketType.HasValue) query = query.Where(z => z.Type == request.RocketType);

            return query.ProjectTo<RocketModel>(mapper.ConfigurationProvider).AsAsyncEnumerable();
        }
    }
}
