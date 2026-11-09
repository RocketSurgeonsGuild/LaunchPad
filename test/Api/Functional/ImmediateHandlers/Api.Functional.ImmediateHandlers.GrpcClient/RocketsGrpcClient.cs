using Api.Functional.ImmediateHandlers.Contracts;
using Rocket.Surgery.LaunchPad.Api;

namespace Api.Functional.ImmediateHandlers.GrpcClient;

[Operation<GetRocket.Request>]
[Operation<ListRockets.Request>]
[Operation<CreateRocket.Request>]
[Operation<EditRocket.Request>]
[Operation<DeleteRocket.Request>]
public partial class RocketsGrpcClient;
