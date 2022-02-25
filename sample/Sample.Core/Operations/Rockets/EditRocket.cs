using AutoMapper;
using FluentValidation;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Domain;
using Sample.Core.Models;

namespace Sample.Core.Operations.Rockets;

[PublicAPI]
public static partial class EditRocket
{
    public record Request : IRequest<RocketModel>
    {
        public Guid Id { get; init; }

        // TODO: Make generator that can be used to create a writable view model
        public string SerialNumber { get; set; } = null!;

        // TODO: Make generator that can be used to create a writable view model
        public RocketType Type { get; set; }
    }

    private class Mapper : Profile
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

    private class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id)
               .NotEmpty()
               .NotNull();

            RuleFor(x => x.Type)
               .NotNull()
               .IsInEnum();

            RuleFor(x => x.SerialNumber)
               .NotNull()
               .MinimumLength(10)
               .MaximumLength(30);
        }
    }

    private class Handler : IRequestHandler<Request, RocketModel>
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
