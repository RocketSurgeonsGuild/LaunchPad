using Api.Functional.ImmediateHandlers.Contracts;

namespace Api.Functional.ImmediateHandlers.Client;

[Operation<GetRocket.Request>]
[Operation<ListRockets.Request>]
[Operation<CreateRocket.Request>]
[Operation<EditRocket.Request>]
[Operation<DeleteRocket.Request>]
public partial class RocketsClient;
