using Microsoft.AspNetCore.Mvc;

using Rocket.Surgery.LaunchPad.Api;

using Sample.Core.Operations.LaunchRecords;

namespace Sample.Restful.Controllers;

/// <summary>The launch record operations; the generator writes one action per operation.</summary>
[Route("[controller]")]
[Operation<ListLaunchRecords.Request>(Summary = "Search for launch records")]
[Operation<GetLaunchRecord.Request>(Summary = "Load details of a specific launch record")]
[CreateOperation<CreateLaunchRecord.Request, GetLaunchRecord.Request>(Summary = "Create a new launch record")]
[Operation<EditLaunchRecord.Request>(Summary = "Update a given launch record")]
[Operation<EditLaunchRecord.PatchRequest>(Summary = "Update a given launch record")]
[Operation<DeleteLaunchRecord.Request>(Summary = "Remove a launch record")]
public partial class LaunchRecordController;
