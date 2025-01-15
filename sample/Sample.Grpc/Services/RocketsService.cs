using FluentValidation;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using MediatR;

using Riok.Mapperly.Abstractions;

using Sample.Core.Operations.Rockets;
using NodaTimeMapper = Rocket.Surgery.LaunchPad.Mapping.NodaTimeMapper;

namespace Sample.Grpc.Services;

[Mapper]
[UseStaticMapper(typeof(NodaTimeMapper))]
[UseStaticMapper(typeof(WellKnownGrpcTypesMapper))]
public partial class RocketsService(IMediator mediator) : Rockets.RocketsBase
{
    public static partial CreateRocket.Request Map(CreateRocketRequest request);
    public static partial CreateRocketResponse Map(CreateRocket.Response request);
    public static partial GetRocket.Request Map(GetRocketRequest request);
    public static partial EditRocket.Request Map(UpdateRocketRequest request);
    public static partial ListRockets.Request Map(ListRocketsRequest request);
    public static partial DeleteRocket.Request Map(DeleteRocketRequest request);
    public static partial RocketModel Map(Core.Models.RocketModel request);

    public override async Task<CreateRocketResponse> CreateRocket(CreateRocketRequest request, ServerCallContext context) => Map(await mediator.Send(Map(request), context.CancellationToken));

    public override async Task<RocketModel> EditRocket(UpdateRocketRequest request, ServerCallContext context) => Map(await mediator.Send(Map(request), context.CancellationToken));

    public override async Task<Empty> DeleteRocket(DeleteRocketRequest request, ServerCallContext context)
    {
        await mediator.Send(Map(request), context.CancellationToken);
        return new();
    }

    public override async Task<RocketModel> GetRockets(GetRocketRequest request, ServerCallContext context) => Map(await mediator.Send(Map(request), context.CancellationToken));

    public override async Task ListRockets(ListRocketsRequest request, IServerStreamWriter<RocketModel> responseStream, ServerCallContext context)
    {
        await foreach (var item in mediator.CreateStream(Map(request), context.CancellationToken))
        {
            await responseStream.WriteAsync(Map(item));
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
        public GetRocketRequestValidator() => RuleFor(x => x.Id)
            .NotEmpty()
            .NotNull();
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
        public DeleteRocketRequestValidator() => RuleFor(x => x.Id)
            .NotEmpty()
            .NotNull();
    }
}
