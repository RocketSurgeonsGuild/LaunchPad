using System.Text.Json.Serialization;
using Api.Functional.ChannelMediator.Contracts;

namespace Api.Functional.ChannelMediator.Server;

/// <summary>The JSON metadata of the bodies and responses the generated Minimal API endpoints read and write (LPAD1030).</summary>
[JsonSerializable(typeof(RocketModel))]
[JsonSerializable(typeof(CreateRocket.Request), TypeInfoPropertyName = "CreateRocketRequest")]
[JsonSerializable(typeof(CreateRocket.Response), TypeInfoPropertyName = "CreateRocketResponse")]
[JsonSerializable(typeof(EditRocket.Request), TypeInfoPropertyName = "EditRocketRequest")]
internal sealed partial class ServerJsonContext : JsonSerializerContext;
