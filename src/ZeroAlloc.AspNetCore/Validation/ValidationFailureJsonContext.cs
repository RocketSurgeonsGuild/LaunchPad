using System.Text.Json.Serialization;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Validation;

/// <summary>
///     The metadata of the problem details this package writes, so they serialize without reflection. The <c>ZeroAllocProblemDetailsJoint</c>
///     chains it behind the application's own resolvers.
/// </summary>
[JsonSerializable(typeof(ValidationFailureProblemDetails))]
internal sealed partial class ValidationFailureJsonContext : JsonSerializerContext;
