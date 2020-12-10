using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Domain;
using Sample.Core.Models;
using System.ComponentModel;

namespace Sample.Core.Operations.Rockets
{
    [PublicAPI]
    public static partial class EditRocket
    {
        public record Model
        {
            public string SerialNumber { get; set; } // TODO: Make generator that can be used to create a writable view model
            public RocketType Type { get; set; } // TODO: Make generator that can be used to create a writable view model
        }

        [InheritFrom(typeof(Model))]
        public partial record Request : Model, IRequest<RocketModel>
        {
            public Guid Id { get; init; }
        }

        class Mapper : Profile
        {
            public Mapper()
            {
                CreateMap<Request, ReadyRocket>()
                   .ForMember(x => x.Id, x => x.Ignore())
                   .ForMember(x => x.LaunchRecords, x => x.Ignore())
                    ;
                CreateMap<RocketModel, Request>()
                   .ForMember(x => x.SerialNumber, x => x.MapFrom(z => z.Sn))
                    ;
            }
        }

        class ModelValidator : AbstractValidator<Model>
        {
            public ModelValidator()
            {
                RuleFor(x => x.Type)
                   .NotNull()
                   .IsInEnum();

                RuleFor(x => x.SerialNumber)
                   .NotNull()
                   .MinimumLength(10)
                   .MaximumLength(30);
            }
        }

        class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                Include(new ModelValidator());
                RuleFor(x => x.Id)
                   .NotEmpty()
                   .NotNull();
            }
        }

        class Handler : IRequestHandler<Request, RocketModel>
        {
            private readonly RocketDbContext _dbContext;
            private readonly IMapper _mapper;

            public Handler(RocketDbContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }

            public async Task<RocketModel> Handle(Request request, CancellationToken cancellationToken)
            {
                var rocket = await _dbContext.Rockets.FindAsync(new object[] { request.Id }, cancellationToken).ConfigureAwait(false);
                if (rocket == null)
                {
                    throw new NotFoundException();
                }

                _mapper.Map(request, rocket);
                _dbContext.Update(rocket);
                await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return _mapper.Map<RocketModel>(rocket);
            }
        }
    }
}