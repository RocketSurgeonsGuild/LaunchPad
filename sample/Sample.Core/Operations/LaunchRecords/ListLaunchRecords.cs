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
    public record Request : IRequest<IEnumerable<LaunchRecordModel>>
    {
    }

    private class Validator : AbstractValidator<Request>
    {
    }

    private class Handler : IRequestHandler<Request, IEnumerable<LaunchRecordModel>>
    {
        private readonly RocketDbContext _dbContext;
        private readonly IMapper _mapper;

        public Handler(RocketDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LaunchRecordModel>> Handle(Request request, CancellationToken cancellationToken)
        {
            return (
                    await _dbContext.LaunchRecords
                                    .Include(x => x.Rocket)
                                    .ProjectTo<LaunchRecordModel>(_mapper.ConfigurationProvider)
                                    .ToListAsync(cancellationToken)
                )
               .ToArray();
        }
    }
}
