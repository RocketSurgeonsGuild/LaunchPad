using Api.Functional.Mediator.Contracts;
using Rocket.Surgery.LaunchPad.Api;

namespace Api.Functional.Mediator.GraphQLClientHost;

[Operation<CreateRocket.Request>]
[Operation<EditRocket.Request>]
[Operation<DeleteRocket.Request>]
public partial class RocketsGraphQLClient;
