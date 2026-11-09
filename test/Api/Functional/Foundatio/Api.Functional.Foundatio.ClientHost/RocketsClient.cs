using Api.Functional.Foundatio.Contracts;
using Rocket.Surgery.LaunchPad.Api;

namespace Api.Functional.Foundatio.ClientHost;

[Operation<GetRocket.Request>]
[Operation<ListRockets.Request>]
[Operation<CreateRocket.Request>]
[Operation<EditRocket.Request>]
[Operation<DeleteRocket.Request>]
public partial class RocketsClient;
