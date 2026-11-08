using Api.Functional.MediatR.Contracts;
using Rocket.Surgery.LaunchPad.Api;

namespace Api.Functional.MediatR.Client;

[Operation<GetRocket.Request>]
[Operation<ListRockets.Request>]
[Operation<CreateRocket.Request>]
[Operation<EditRocket.Request>]
[Operation<DeleteRocket.Request>]
public partial class RocketsClient;

// The versioned server host's client: it sends each operation's version as api-version. No handlers: RocketsClient handles these requests.
[OperationHost(Group = "versioned-rockets", Version = "1.0", Handlers = false)]
[Operation<GetRocket.Request>(Version = "2.0")]
[Operation<CreateRocket.Request>]
public partial class VersionedRocketsClient;
