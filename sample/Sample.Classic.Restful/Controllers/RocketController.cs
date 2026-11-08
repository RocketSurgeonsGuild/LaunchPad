using Microsoft.AspNetCore.Mvc;

using Sample.Core.Operations.Rockets;

namespace Sample.Classic.Restful.Controllers;

/// <summary>The rocket operations; the generator writes one action per operation.</summary>
[Route("[controller]")]
[Operation<ListRockets.Request>(Summary = "Search for rockets")]
[Operation<GetRocket.Request>(Summary = "Load details of a specific rocket")]
[Operation<CreateRocket.Request>(Summary = "Create a new rocket")]
[Operation<EditRocket.Request>(Summary = "Update a given rocket")]
[Operation<EditRocket.PatchRequest>(Summary = "Update a given rocket")]
[Operation<DeleteRocket.Request>(Name = "RemoveRocket", Summary = "Remove a rocket")]
[Operation<GetRocketLaunchRecords.Request>(Summary = "Get the launch records for a given rocket")]
[Operation<GetRocketLaunchRecord.Request>(Route = "{id}/launch-records/{launchRecordId}", Summary = "Get a specific launch record for a given rocket")]
public partial class RocketController;
