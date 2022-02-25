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
    // TODO: Paging model!
    public record Request(RocketType? RocketType) : IStreamRequest<LaunchRecordModel>;

    private class Validator : AbstractValidator<Request>
    {
    }

    private class Handler : IStreamRequestHandler<Request, LaunchRecordModel>
    {
        private readonly RocketDbContext _dbContext;
        private readonly IMapper _mapper;

        public Handler(RocketDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IAsyncEnumerable<LaunchRecordModel> Handle(Request request, CancellationToken cancellationToken)
        {
            var query = _dbContext.LaunchRecords
                                  .Include(x => x.Rocket)
                                  .AsQueryable();
            if (request.RocketType.HasValue)
            {
                query = query.Where(z => z.Rocket.Type == request.RocketType);
            }

            return query
                  .ProjectTo<LaunchRecordModel>(_mapper.ConfigurationProvider)
                  .ToAsyncEnumerable();
        }
    }
}
