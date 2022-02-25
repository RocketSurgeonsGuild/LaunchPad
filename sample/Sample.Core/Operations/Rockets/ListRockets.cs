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
    public record Request(RocketType? RocketType) : IStreamRequest<RocketModel>;

    private class Validator : AbstractValidator<Request>
    {
    }

    private class Handler : IStreamRequestHandler<Request, RocketModel>
    {
        private readonly RocketDbContext _dbContext;
        private readonly IMapper _mapper;

        public Handler(RocketDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IAsyncEnumerable<RocketModel> Handle(Request request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Rockets.AsQueryable();
            if (request.RocketType.HasValue)
            {
                query = query.Where(z => z.Type == request.RocketType);
            }

            return query.ProjectTo<RocketModel>(_mapper.ConfigurationProvider).AsAsyncEnumerable();
        }
    }
}
