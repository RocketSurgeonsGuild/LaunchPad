using Api.Functional.ImmediateHandlers.Contracts;
using Rocket.Surgery.LaunchPad.Api;

namespace Api.Functional.ImmediateHandlers.GraphQLClient;

[Operation<CreateRocket.Request>]
[Operation<EditRocket.Request>]
[Operation<DeleteRocket.Request>]
public partial class RocketsGraphQLClient;
