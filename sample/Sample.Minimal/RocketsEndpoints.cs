using Rocket.Surgery.LaunchPad.Api;

using Sample.Core.Operations.Rockets;

namespace Sample.Minimal;

/// <summary>The rocket operations, mapped by the generator to /rockets with app.MapRocketsEndpoints().</summary>
[OperationHost(RoutePrefix = "")]
[Operation<ListRockets.Request>]
[Operation<GetRocket.Request>]
[Operation<CreateRocket.Request>]
[Operation<EditRocket.Request>]
[Operation<EditRocket.PatchRequest>]
[Operation<DeleteRocket.Request>]
internal static partial class RocketsEndpoints;
