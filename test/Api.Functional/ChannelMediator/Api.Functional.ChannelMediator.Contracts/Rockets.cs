using System.Runtime.Serialization;
using ChannelMediator;
namespace Api.Functional.ChannelMediator.Contracts;

[DataContract] public sealed record RocketModel([property: DataMember(Order = 1)] Guid Id, [property: DataMember(Order = 2)] string Name);

public static class GetRocket { [DataContract] public sealed record Request([property: DataMember(Order = 1)] Guid Id) : IRequest<RocketModel?>; }

public static class ListRockets { [DataContract] public sealed record Request([property: DataMember(Order = 1)] string? Name) : IStreamRequest<RocketModel>; }

public static class CreateRocket
{
    [DataContract] public sealed record Request([property: DataMember(Order = 1)] string Name) : IRequest<Response>;
    [Creates(typeof(GetRocket.Request))][DataContract] public sealed record Response([property: DataMember(Order = 1)] Guid Id);
}

public static class EditRocket { [DataContract] public sealed record Request([property: DataMember(Order = 1)] Guid Id, [property: DataMember(Order = 2)] string Name) : IRequest<RocketModel>; }

public static class DeleteRocket { [DataContract] public sealed record Request([property: DataMember(Order = 1)] Guid Id) : IRequest; }
