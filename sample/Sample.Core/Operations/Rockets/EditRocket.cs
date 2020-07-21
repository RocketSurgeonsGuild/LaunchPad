using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Rocket.Surgery.LaunchPad.Extensions;
using Sample.Core.Domain;
using Sample.Core.Models;

namespace Sample.Core.Operations.Rockets
{
    [PublicAPI]
    public static class EditRocket
    {
        public static Request CreateRequest(Guid id, Model model, IMapper mapper) => mapper.Map(model, new Request(id));

        public class Model
        {
            public string SerialNumber { get; set; }
            public RocketType Type { get; set; }
        }

        public class Request : Model, IRequest<RocketModel>
        {
            public Guid Id { get; }

            public Request(Guid id)
            {
                Id = id;
            }

            private Request() { }
        }

        class Mapper : Profile
        {
            public Mapper()
            {
                CreateMap<Request, ReadyRocket>()
                   .ForMember(x => x.Id, x => x.Ignore())
                   .ForMember(x => x.LaunchRecords, x => x.Ignore())
                    ;
                CreateMap<Model, Request>()
                    ;
                CreateMap<RocketModel, Model>()
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