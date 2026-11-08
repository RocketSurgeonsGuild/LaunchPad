using System.Runtime.Serialization;
using Foundatio.Mediator;

namespace Api.Functional.Foundatio.Contracts;

[DataContract] public sealed record RocketModel([property: DataMember(Order = 1)] Guid Id, [property: DataMember(Order = 2)] string Name);

public static class GetRocket { [DataContract] public sealed record Request([property: DataMember(Order = 1)] Guid Id) : IRequest<RocketModel?>; }

// Foundatio's markers are optional: a stream has none, so its response is named.
public static class ListRockets
{
    [ApiOperation(Response = typeof(IAsyncEnumerable<RocketModel>))]
    [DataContract] public sealed record Request([property: DataMember(Order = 1)] string? Name);
}

public static class CreateRocket
{
    [DataContract] public sealed record Request([property: DataMember(Order = 1)] string Name) : IRequest<Response>;
    [Creates(typeof(GetRocket.Request))][DataContract] public sealed record Response([property: DataMember(Order = 1)] Guid Id);
}

public static class EditRocket { [DataContract] public sealed record Request([property: DataMember(Order = 1)] Guid Id, [property: DataMember(Order = 2)] string Name) : IRequest<RocketModel>; }

public static class DeleteRocket { [DataContract] public sealed record Request([property: DataMember(Order = 1)] Guid Id) : IRequest<bool>; }
