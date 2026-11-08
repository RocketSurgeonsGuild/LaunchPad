using Api.Functional.MediatR.Contracts;

namespace Api.Functional.MediatR.GrpcClient;

[Operation<GetRocket.Request>]
[Operation<ListRockets.Request>]
[Operation<CreateRocket.Request>]
[Operation<EditRocket.Request>]
[Operation<DeleteRocket.Request>]
public partial class RocketsGrpcClient;
