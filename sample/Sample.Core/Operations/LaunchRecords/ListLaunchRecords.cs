using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sample.Core.Domain;
using Sample.Core.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.Core.Operations.LaunchRecords
{
    [PublicAPI]
    public static class ListLaunchRecords
    {
        // TODO: Paging model!
        public record Request : IRequest<IEnumerable<LaunchRecordModel>> { }

        class Validator : AbstractValidator<Request>
        { }

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
                       .ProjectTo<LaunchRecordModel>(_mapper.ConfigurationProvider)
                       .ToListAsync(cancellationToken)
                )
               .ToArray();
        }
    }
}