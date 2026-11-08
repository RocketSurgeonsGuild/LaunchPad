using Api.Functional.Foundatio.Contracts;

namespace Api.Functional.Foundatio.GraphQLClientHost;

[Operation<CreateRocket.Request>]
[Operation<EditRocket.Request>]
[Operation<DeleteRocket.Request>]
public partial class RocketsGraphQLClient;
