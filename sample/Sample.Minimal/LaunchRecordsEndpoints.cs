using Sample.Core.Operations.LaunchRecords;

namespace Sample.Minimal;

/// <summary>The launch record operations, mapped by the generator to /launch-records with app.MapLaunchRecordsEndpoints().</summary>
[OperationHost(RoutePrefix = "")]
[Operation<ListLaunchRecords.Request>]
[Operation<GetLaunchRecord.Request>]
[Operation<CreateLaunchRecord.Request>]
[Operation<EditLaunchRecord.Request>]
[Operation<EditLaunchRecord.PatchRequest>]
[Operation<DeleteLaunchRecord.Request>]
internal static partial class LaunchRecordsEndpoints;
