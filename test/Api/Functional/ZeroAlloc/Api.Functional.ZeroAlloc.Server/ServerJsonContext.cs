using System.Text.Json.Serialization;
using Api.Functional.ZeroAlloc.Contracts;

namespace Api.Functional.ZeroAlloc.Server;

/// <summary>The JSON metadata of the bodies and responses the generated Minimal API endpoints read and write (LPAD1030).</summary>
[JsonSerializable(typeof(RocketModel))]
[JsonSerializable(typeof(CreateRocket.Request), TypeInfoPropertyName = "CreateRocketRequest")]
[JsonSerializable(typeof(CreateRocket.Response), TypeInfoPropertyName = "CreateRocketResponse")]
[JsonSerializable(typeof(EditRocket.Request), TypeInfoPropertyName = "EditRocketRequest")]
[JsonSerializable(typeof(LaunchRocket.Request), TypeInfoPropertyName = "LaunchRocketRequest")]
[JsonSerializable(typeof(ScheduleLaunch.Request), TypeInfoPropertyName = "ScheduleLaunchRequest")]
internal sealed partial class ServerJsonContext : JsonSerializerContext;
