using Api.Functional.Mediator.Contracts;

namespace Api.Functional.Mediator.GrpcClientHost;

[Operation<GetRocket.Request>]
[Operation<ListRockets.Request>]
[Operation<CreateRocket.Request>]
[Operation<EditRocket.Request>]
[Operation<DeleteRocket.Request>]
public partial class RocketsGrpcClient;
