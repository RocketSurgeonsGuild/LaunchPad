using Api.Functional.ChannelMediator.Contracts;

namespace Api.Functional.ChannelMediator.GrpcClient;

[Operation<GetRocket.Request>]
[Operation<ListRockets.Request>]
[Operation<CreateRocket.Request>]
[Operation<EditRocket.Request>]
[Operation<DeleteRocket.Request>]
public partial class RocketsGrpcClient;
