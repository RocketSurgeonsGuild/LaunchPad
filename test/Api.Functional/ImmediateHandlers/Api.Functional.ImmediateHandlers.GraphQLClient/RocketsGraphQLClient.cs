using Api.Functional.ImmediateHandlers.Contracts;

namespace Api.Functional.ImmediateHandlers.GraphQLClient;

[Operation<CreateRocket.Request>]
[Operation<EditRocket.Request>]
[Operation<DeleteRocket.Request>]
public partial class RocketsGraphQLClient;
