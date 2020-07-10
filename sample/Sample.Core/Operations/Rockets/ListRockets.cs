using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sample.Core.Domain;
using Sample.Core.Models;

namespace Sample.Core.Operations.Rockets
{
    [PublicAPI]
    public static class ListRockets
    {
        // TODO: Paging model!
        public class Request : IRequest<IEnumerable<RocketModel>> { }

        class Validator : AbstractValidator<Request>
        {
            public Validator()
            {

            }
        }

        class Handler : IRequestHandler<Request, IEnumerable<RocketModel>>
        {
            private readonly RocketDbContext _dbContext;
            private readonly IMapper _mapper;

            public Handler(RocketDbContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }

            public async Task<IEnumerable<RocketModel>> Handle(Request request, CancellationToken cancellationToken)
            {
                return await _dbContext.Rockets
                   .ProjectTo<RocketModel>(_mapper.ConfigurationProvider)
                   .ToListAsync(cancellationToken)
                   .ConfigureAwait(false);
            }
        }
    }
}