﻿using AutoMapper;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Domain;
using Sample.Core.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.Core.Operations.LaunchRecords
{
    [PublicAPI]
    public static class GetLaunchRecord
    {
        public record Request : IRequest<LaunchRecordModel>
        {
            public Guid Id { get; init; }
        }

        class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Id)
                   .NotEmpty()
                   .NotNull();
            }
        }

        class Handler : IRequestHandler<Request, LaunchRecordModel>
        {
            private readonly RocketDbContext _dbContext;
            private readonly IMapper _mapper;

            public Handler(RocketDbContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }

            public async Task<LaunchRecordModel> Handle(Request request, CancellationToken cancellationToken)
            {
                var rocket = await _dbContext.LaunchRecords
                   .Include(x => x.Rocket)
                   .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                if (rocket == null)
                {
                    throw new NotFoundException();
                }

                return _mapper.Map<LaunchRecordModel>(rocket);
            }
        }
    }
}