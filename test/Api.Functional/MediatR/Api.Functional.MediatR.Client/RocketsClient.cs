using Api.Functional.MediatR.Contracts;
using Rocket.Surgery.LaunchPad.Api;

namespace Api.Functional.MediatR.Client;

[Operation<GetRocket.Request>]
[Operation<ListRockets.Request>]
[Operation<CreateRocket.Request>]
[Operation<EditRocket.Request>]
[Operation<DeleteRocket.Request>]
public partial class RocketsClient;
