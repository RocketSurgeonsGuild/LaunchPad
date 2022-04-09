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
    /// <summary>
    ///     The edit operation to update a rocket
    /// </summary>
    public record Request : IRequest<RocketModel>
    {
        /// <summary>
        ///     The rocket id
        /// </summary>
        public RocketId Id { get; init; }

        /// <summary>
        ///     The serial number of the rocket
        /// </summary>
        public string SerialNumber { get; set; } = null!; // TODO: Make generator that can be used to create a writable view model

        /// <summary>
        ///     The type of the rocket
        /// </summary>
        public RocketType Type { get; set; } // TODO: Make generator that can be used to create a writable view model
    }

    public partial record PatchRequest : IRequest<RocketModel>, IPropertyTracking<Request>
    {
        /// <summary>
        ///     The rocket id
        /// </summary>
        public RocketId Id { get; init; }
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

    private class Handler : PatchHandlerBase<Request, PatchRequest, RocketModel>, IRequestHandler<Request, RocketModel>
    {
        private readonly RocketDbContext _dbContext;
        private readonly IMapper _mapper;

        public Handler(RocketDbContext dbContext, IMapper mapper, IMediator mediator) : base(mediator)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        private async Task<ReadyRocket?> GetRocket(RocketId id, CancellationToken cancellationToken)
        {
            var rocket = await _dbContext.Rockets.FindAsync(new object[] { id }, cancellationToken)
                                         .ConfigureAwait(false);
            if (rocket == null)
            {
                throw new NotFoundException();
            }

            return rocket;
        }

        protected override async Task<Request> GetRequest(PatchRequest patchRequest, CancellationToken cancellationToken)
        {
            var rocket = await GetRocket(patchRequest.Id, cancellationToken);
            return _mapper.Map<Request>(_mapper.Map<RocketModel>(rocket));
        }

        public async Task<RocketModel> Handle(Request request, CancellationToken cancellationToken)
        {
            var rocket = await GetRocket(request.Id, cancellationToken);

            _mapper.Map(request, rocket);
            _dbContext.Update(rocket);
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return _mapper.Map<RocketModel>(rocket);
        }
    }
}
