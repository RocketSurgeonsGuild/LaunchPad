using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sample.Core.Domain;
using Sample.Core.Models;

namespace Sample.Core.Operations.LaunchRecords
{
    [PublicAPI]
    public static class ListLaunchRecords
    {
        // TODO: Paging model!
        public class Request : IRequest<IEnumerable<LaunchRecordModel>> { }

        class Handler : IRequestHandler<Request, IEnumerable<LaunchRecordModel>>
        {
            private readonly RocketDbContext _dbContext;
            private readonly IMapper _mapper;

            public Handler(RocketDbContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }

            public async Task<IEnumerable<LaunchRecordModel>> Handle(Request request, CancellationToken cancellationToken) => (
                    await _dbContext.LaunchRecords
                       .Include(x => x.Rocket)
                       .ToListAsync(cancellationToken)
                )
               .Select(z => _mapper.Map<LaunchRecordModel>(z))
               .ToArray();
        }
    }
}