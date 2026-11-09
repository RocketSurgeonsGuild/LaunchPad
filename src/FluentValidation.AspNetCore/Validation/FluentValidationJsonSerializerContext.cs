using System.Text.Json.Serialization;

using Rocket.Surgery.LaunchPad.Foundation;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Validation;

/// <summary>
///     The metadata for the FluentValidation problem details, so they serialize without reflection. The <c>FluentValidationJoint</c> chains it
///     into the application's JSON options.
/// </summary>
[JsonSerializable(typeof(FluentValidationProblemDetails))]
[JsonSerializable(typeof(ValidationProblemDetailsConverter.AnnotatedProblemDetails))]
[JsonSerializable(typeof(FluentValidationProblemDetail))]
[JsonSerializable(typeof(FluentValidationProblemDetail[]))]
internal sealed partial class FluentValidationJsonSerializerContext : JsonSerializerContext;
