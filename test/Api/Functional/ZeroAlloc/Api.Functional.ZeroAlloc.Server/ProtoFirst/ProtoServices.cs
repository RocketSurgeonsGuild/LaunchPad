using Api.Functional.ZeroAlloc.Contracts;
using Rocket.Surgery.LaunchPad.Api;
using ZeroAlloc.Mapping;

namespace Api.Functional.ZeroAlloc.Server.ProtoFirst;

// Proto-first hosts: each derives the service base Grpc.Tools generated from proto_first.proto, and the generator overrides the rpc of each
// listed operation (matched by name).
[Operation<GetRocket.Request>]
[Operation<ListRockets.Request>]
[Operation<CreateRocket.Request>]
[Operation<EditRocket.Request>]
[Operation<DeleteRocket.Request>]
public partial class ProtoRocketsService : Rockets.RocketsBase;

[Operation<LaunchRocket.Request>]
[Operation<ListLaunches.Request>]
public partial class ProtoLaunchesService : Launches.LaunchesBase;

// The mappings between the proto messages and the operations' requests and responses; ZeroAlloc.Mapping writes them. A proto message has
// settable properties and no constructor parameters, so it's filled in place (the partial void Map overloads). NotReady's mapping puts the
// case in the case-bin trailer; RocketNotFound has none, so only its name and status travel.
[Map<GetRocketRequest, GetRocket.Request>]
[Map<ListRocketsRequest, ListRockets.Request>]
[Map<CreateRocketRequest, CreateRocket.Request>]
[Map<EditRocketRequest, EditRocket.Request>]
[Map<DeleteRocketRequest, DeleteRocket.Request>]
[Map<RocketModel, RocketReply>]
[Map<CreateRocket.Response, CreateRocketReply>]
[Map<LaunchRocketRequest, LaunchRocket.Request>]
[Map<ListLaunchesRequest, ListLaunches.Request>]
[Map<NotReady, NotReadyError>]
public static partial class ProtoMappings
{
    // ZeroAlloc.Mapping's update-in-place form doesn't convert a Guid to the proto's string (it would assign one to the other), so the ids
    // are set by the AfterMap hooks instead, after a placeholder value.
    [MapValue("Id", "")]
    public static partial void Map(RocketModel src, RocketReply existingDst);

    [MapValue("Id", "")]
    public static partial void Map(CreateRocket.Response src, CreateRocketReply existingDst);

    public static partial void Map(NotReady src, NotReadyError existingDst);

    [AfterMap]
    public static void RocketId(RocketModel src, RocketReply dst) => dst.Id = src.Id.ToString();

    [AfterMap]
    public static void CreatedId(CreateRocket.Response src, CreateRocketReply dst) => dst.Id = src.Id.ToString();
}
