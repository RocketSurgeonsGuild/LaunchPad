using Api.Functional.Mediator.Contracts;
using Rocket.Surgery.LaunchPad.Api;

namespace Api.Functional.Mediator.ClientHost;

[Operation<GetRocket.Request>]
[Operation<ListRockets.Request>]
[CreateOperation<CreateRocket.Request, GetRocket.Request>]
[Operation<EditRocket.Request>]
[Operation<DeleteRocket.Request>]
public partial class RocketsClient;
