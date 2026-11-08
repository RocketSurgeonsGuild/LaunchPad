using Api.Functional.ZeroAlloc.Contracts;
using Rocket.Surgery.LaunchPad.Api;

namespace Api.Functional.ZeroAlloc.Client;

[Operation<GetRocket.Request>]
[Operation<ListRockets.Request>]
[CreateOperation<CreateRocket.Request, GetRocket.Request>]
[Operation<EditRocket.Request>]
[Operation<DeleteRocket.Request>]
public partial class RocketsClient;
