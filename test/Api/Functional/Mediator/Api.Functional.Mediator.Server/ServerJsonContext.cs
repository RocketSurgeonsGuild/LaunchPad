using System.Text.Json.Serialization;
using Api.Functional.Mediator.Contracts;

namespace Api.Functional.Mediator.Server;

/// <summary>The JSON metadata of the bodies and responses the generated Minimal API endpoints read and write (LPAD1030).</summary>
[JsonSerializable(typeof(RocketModel))]
[JsonSerializable(typeof(CreateRocket.Request), TypeInfoPropertyName = "CreateRocketRequest")]
[JsonSerializable(typeof(CreateRocket.Response), TypeInfoPropertyName = "CreateRocketResponse")]
[JsonSerializable(typeof(EditRocket.Request), TypeInfoPropertyName = "EditRocketRequest")]
internal sealed partial class ServerJsonContext : JsonSerializerContext;
