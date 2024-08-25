using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;

namespace Sample.Grpc.Services;

public class RocketsService(IMediator mediator, IMapper mapper) : Rockets.RocketsBase
{
    public override async Task<CreateRocketResponse> CreateRocket(CreateRocketRequest request, ServerCallContext context)
    {
        var response = await mediator.Send(mapper.Map<CreateRocket.Request>(request), context.CancellationToken);
        return mapper.Map<CreateRocketResponse>(response);
    }

    public override async Task<RocketModel> EditRocket(UpdateRocketRequest request, ServerCallContext context)
    {
        var response = await mediator.Send(mapper.Map<EditRocket.Request>(request), context.CancellationToken);
        return mapper.Map<RocketModel>(response);
    }

    public override async Task<Empty> DeleteRocket(DeleteRocketRequest request, ServerCallContext context)
    {
        await mediator.Send(mapper.Map<DeleteRocket.Request>(request), context.CancellationToken);
        return new Empty();
    }

    public override async Task<RocketModel> GetRockets(GetRocketRequest request, ServerCallContext context)
    {
        var response = await mediator.Send(mapper.Map<GetRocket.Request>(request), context.CancellationToken);
        return mapper.Map<RocketModel>(response);
    }

    public override async Task ListRockets(ListRocketsRequest request, IServerStreamWriter<RocketModel> responseStream, ServerCallContext context)
    {
        var mRequest = mapper.Map<ListRockets.Request>(request);
        await foreach (var item in mediator.CreateStream(mRequest, context.CancellationToken))
        {
            await responseStream.WriteAsync(mapper.Map<RocketModel>(item));
        }
    }

    [UsedImplicitly]
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
            CreateMap<RocketId, string>().ConvertUsing(x => x.Value.ToString());
            CreateMap<string, RocketId>().ConvertUsing(x => new RocketId(Guid.Parse(x)));
            CreateMap<LaunchRecordId, string>().ConvertUsing(x => x.Value.ToString());
            CreateMap<string, LaunchRecordId>().ConvertUsing(x => new LaunchRecordId(Guid.Parse(x)));

            CreateMap<NullableRocketType?, Core.Domain.RocketType>().ConvertUsing(
                static ts =>
                    ts != null && ts.KindCase == NullableRocketType.KindOneofCase.Data
                        ? (Core.Domain.RocketType)ts.Data
                        : default
            );
            CreateMap<Core.Domain.RocketType, NullableRocketType>().ConvertUsing(
                static ts =>
                    new NullableRocketType { Data = (RocketType)ts }
            );
        }
    }

    [UsedImplicitly]
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

    [UsedImplicitly]
    private class GetRocketRequestValidator : AbstractValidator<GetRocketRequest>
    {
        public GetRocketRequestValidator()
        {
            RuleFor(x => x.Id)
               .NotEmpty()
               .NotNull();
        }
    }

    [UsedImplicitly]
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

    [UsedImplicitly]
    private class ListRocketsRequestValidator : AbstractValidator<ListRocketsRequest>;

    [UsedImplicitly]
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
