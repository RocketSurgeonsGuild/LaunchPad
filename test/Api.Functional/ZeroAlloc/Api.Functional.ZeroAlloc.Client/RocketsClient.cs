using Api.Functional.ZeroAlloc.Contracts;

namespace Api.Functional.ZeroAlloc.Client;

[Operation<GetRocket.Request>]
[Operation<ListRockets.Request>]
[Operation<CreateRocket.Request>]
[Operation<EditRocket.Request>]
[Operation<DeleteRocket.Request>]
public partial class RocketsClient;
