using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Mvc;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Mvc;

/// <summary>
///     The metadata of the problem details MVC answers with (a controller's 400 for a value it can't bind), so the OpenAPI document and the HTTP
///     JSON options describe them without reflection. The <c>MvcSystemJsonTextJoint</c> chains it behind the application's own resolvers.
/// </summary>
[JsonSerializable(typeof(ValidationProblemDetails))]
internal sealed partial class MvcJsonSerializerContext : JsonSerializerContext;
