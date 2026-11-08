using Api.Functional.Mediator.Contracts;

namespace Api.Functional.Mediator.GraphQLClientHost;

[Operation<CreateRocket.Request>]
[Operation<EditRocket.Request>]
[Operation<DeleteRocket.Request>]
public partial class RocketsGraphQLClient;
