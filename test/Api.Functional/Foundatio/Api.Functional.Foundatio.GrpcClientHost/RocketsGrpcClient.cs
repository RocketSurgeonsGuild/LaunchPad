using Api.Functional.Foundatio.Contracts;

namespace Api.Functional.Foundatio.GrpcClientHost;

[Operation<GetRocket.Request>]
[Operation<ListRockets.Request>]
[Operation<CreateRocket.Request>]
[Operation<EditRocket.Request>]
[Operation<DeleteRocket.Request>]
public partial class RocketsGrpcClient;
