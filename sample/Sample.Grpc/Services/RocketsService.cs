using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Riok.Mapperly.Abstractions;
using Sample.Core.Operations.Rockets;
using ZeroAlloc.Mediator;
using NodaTimeMapper = Sample.Core.Models.NodaTimeMapper;

namespace Sample.Grpc.Services;

[Mapper]
[UseStaticMapper(typeof(NodaTimeMapper))]
[UseStaticMapper(typeof(WellKnownGrpcTypesMapper))]
public partial class RocketsService : Rockets.RocketsBase
{
    private static IMediator Mediator(ServerCallContext context) => context.GetHttpContext().RequestServices.GetRequiredService<IMediator>();

    public static partial CreateRocket.Request Map(CreateRocketRequest request);
    public static partial CreateRocketResponse Map(CreateRocket.Response request);
    public static partial GetRocket.Request Map(GetRocketRequest request);
    public static partial EditRocket.Request Map(UpdateRocketRequest request);
    public static partial ListRockets.Request Map(ListRocketsRequest request);
    public static partial DeleteRocket.Request Map(DeleteRocketRequest request);
    public static partial RocketModel Map(Core.Models.RocketModel request);

    public override async Task<CreateRocketResponse> CreateRocket(CreateRocketRequest request, ServerCallContext context) => Map(await Mediator(context).Send(Map(request), context.CancellationToken));

    public override async Task<RocketModel> EditRocket(UpdateRocketRequest request, ServerCallContext context) => Map(await Mediator(context).Send(Map(request), context.CancellationToken));

    public override async Task<Empty> DeleteRocket(DeleteRocketRequest request, ServerCallContext context)
    {
        await Mediator(context).Send(Map(request), context.CancellationToken);
        return new();
    }

    public override async Task<RocketModel> GetRockets(GetRocketRequest request, ServerCallContext context) => Map(await Mediator(context).Send(Map(request), context.CancellationToken));

    public override async Task ListRockets(ListRocketsRequest request, IServerStreamWriter<RocketModel> responseStream, ServerCallContext context)
    {
        await foreach (var item in Mediator(context).CreateStream(Map(request), context.CancellationToken))
        {
            await responseStream.WriteAsync(Map(item));
        }
    }
}
