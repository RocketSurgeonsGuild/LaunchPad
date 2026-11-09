using System.Runtime.Serialization;
using Rocket.Surgery.LaunchPad.Api;

namespace Api.Functional.ImmediateHandlers.Contracts;

// Immediate.Handlers requests implement nothing: the server reads the response from its [Handler], the client from [ApiOperation(Response)].
[DataContract] public sealed record RocketModel([property: DataMember(Order = 1)] Guid Id, [property: DataMember(Order = 2)] string Name);

public static class GetRocket
{
    [ApiOperation(Response = typeof(RocketModel))]
    [DataContract] public sealed record Request([property: DataMember(Order = 1)] Guid Id);
}

public static class ListRockets
{
    [ApiOperation(Response = typeof(IAsyncEnumerable<RocketModel>))]
    [DataContract] public sealed record Request([property: DataMember(Order = 1)] string? Name);
}

public static class CreateRocket
{
    [ApiOperation(Response = typeof(Response))]
    [DataContract] public sealed record Request([property: DataMember(Order = 1)] string Name);
    [Creates(typeof(GetRocket.Request))][DataContract] public sealed record Response([property: DataMember(Order = 1)] Guid Id);
}

public static class EditRocket
{
    [ApiOperation(Response = typeof(RocketModel))]
    [DataContract] public sealed record Request([property: DataMember(Order = 1)] Guid Id, [property: DataMember(Order = 2)] string Name);
}

public static class DeleteRocket
{
    [DataContract] public sealed record Request([property: DataMember(Order = 1)] Guid Id);
}
