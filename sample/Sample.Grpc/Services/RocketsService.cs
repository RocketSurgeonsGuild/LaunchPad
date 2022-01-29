using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Sample.Core.Operations.Rockets;

namespace Sample.Grpc.Services;

public class RocketsService : Rockets.RocketsBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public RocketsService(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    public override async Task<CreateRocketResponse> CreateRocket(CreateRocketRequest request, ServerCallContext context)
    {
        var response = await _mediator.Send(_mapper.Map<CreateRocket.Request>(request), context.CancellationToken);
        return _mapper.Map<CreateRocketResponse>(response);
    }

    public override async Task<RocketModel> EditRocket(UpdateRocketRequest request, ServerCallContext context)
    {
        var response = await _mediator.Send(_mapper.Map<EditRocket.Request>(request), context.CancellationToken);
        return _mapper.Map<RocketModel>(response);
    }

    public override async Task<Empty> DeleteRocket(DeleteRocketRequest request, ServerCallContext context)
    {
        await _mediator.Send(_mapper.Map<DeleteRocket.Request>(request), context.CancellationToken);
        return new Empty();
    }

    public override async Task<RocketModel> GetRockets(GetRocketRequest request, ServerCallContext context)
    {
        var response = await _mediator.Send(_mapper.Map<GetRocket.Request>(request), context.CancellationToken);
        return _mapper.Map<RocketModel>(response);
    }

    public override async Task<ListRocketsResponse> ListRockets(ListRocketsRequest request, ServerCallContext context)
    {
        var response = await _mediator.Send(_mapper.Map<ListRockets.Request>(request), context.CancellationToken);
        return new ListRocketsResponse
        {
            Results = { response.Select(_mapper.Map<RocketModel>) }
        };
    }

    private class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<CreateRocketRequest, CreateRocket.Request>();
            CreateMap<CreateRocket.Response, CreateRocketResponse>();
            CreateMap<GetRocketRequest, GetRocket.Request>();
            CreateMap<UpdateRocketRequest, EditRocket.Request>();
            CreateMap<ListRocketsRequest, ListRockets.Request>();
            CreateMap<DeleteRocketRequest, DeleteRocket.Request>();
            CreateMap<Core.Models.RocketModel, RocketModel>();
        }
    }

    private class CreateRocketRequestValidator : AbstractValidator<CreateRocketRequest>
    {
        public CreateRocketRequestValidator()
        {
            RuleFor(x => x.Type)
               .IsInEnum();

            RuleFor(x => x.SerialNumber)
               .NotEmpty()
               .MinimumLength(10)
               .MaximumLength(30);
        }
    }

    private class GetRocketRequestValidator : AbstractValidator<GetRocketRequest>
    {
        public GetRocketRequestValidator()
        {
            RuleFor(x => x.Id)
               .NotEmpty()
               .NotNull();
        }
    }

    private class UpdateRocketRequestValidator : AbstractValidator<UpdateRocketRequest>
    {
        public UpdateRocketRequestValidator()
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

    private class ListRocketsRequestValidator : AbstractValidator<ListRocketsRequest>
    {
    }

    private class DeleteRocketRequestValidator : AbstractValidator<DeleteRocketRequest>
    {
        public DeleteRocketRequestValidator()
        {
            RuleFor(x => x.Id)
               .NotEmpty()
               .NotNull();
        }
    }
}
