using Api.Functional.ChannelMediator.Contracts;
using Rocket.Surgery.LaunchPad.Api;

namespace Api.Functional.ChannelMediator.GraphQLClient;

[Operation<CreateRocket.Request>]
[Operation<EditRocket.Request>]
[Operation<DeleteRocket.Request>]
public partial class RocketsGraphQLClient;
