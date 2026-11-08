using System.Text.Json.Serialization;
using Mediator;
namespace Api.Aot.Contracts;

public sealed record RocketModel(Guid Id, string Name);
public static class GetRocket { public sealed record Request(Guid Id) : IQuery<RocketModel?>; }
public static class ListRockets { public sealed record Request(string? Name) : IStreamQuery<RocketModel>; }
public static class CreateRocket { public sealed record Request(string Name) : ICommand<Response>; public sealed record Response(Guid Id); }
public static class EditRocket { public sealed record Request(Guid Id, string Name) : ICommand<RocketModel>; }
public static class DeleteRocket { public sealed record Request(Guid Id) : ICommand; }

[JsonSerializable(typeof(RocketModel)), JsonSerializable(typeof(CreateRocket.Request), TypeInfoPropertyName = "CreateRocketRequest"), JsonSerializable(typeof(CreateRocket.Response), TypeInfoPropertyName = "CreateRocketResponse"), JsonSerializable(typeof(EditRocket.Request), TypeInfoPropertyName = "EditRocketRequest")]
[JsonSourceGenerationOptions(System.Text.Json.JsonSerializerDefaults.Web)]
// LaunchPad-style nested X.Request types share a simple name, so each needs its own TypeInfoPropertyName (otherwise SYSLIB1031 skips it).
public partial class RocketsJsonContext : JsonSerializerContext;
