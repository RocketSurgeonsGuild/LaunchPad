using Api.Functional.MediatR.Contracts;

namespace Api.Functional.MediatR.GraphQLClient;

[Operation<CreateRocket.Request>]
[Operation<EditRocket.Request>]
[Operation<DeleteRocket.Request>]
public partial class RocketsGraphQLClient;
