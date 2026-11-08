using System.Runtime.Serialization;
using Mediator;
namespace Api.Functional.Mediator.Contracts;

[DataContract] public sealed record RocketModel([property: DataMember(Order = 1)] Guid Id, [property: DataMember(Order = 2)] string Name);

public static class GetRocket { [DataContract] public sealed record Request([property: DataMember(Order = 1)] Guid Id) : IQuery<RocketModel?>; }

public static class ListRockets { [DataContract] public sealed record Request([property: DataMember(Order = 1)] string? Name) : IStreamQuery<RocketModel>; }

public static class CreateRocket
{
    [DataContract] public sealed record Request([property: DataMember(Order = 1)] string Name) : ICommand<Response>;
    [DataContract] public sealed record Response([property: DataMember(Order = 1)] Guid Id);
}

public static class EditRocket { [DataContract] public sealed record Request([property: DataMember(Order = 1)] Guid Id, [property: DataMember(Order = 2)] string Name) : ICommand<RocketModel>; }

public static class DeleteRocket { [DataContract] public sealed record Request([property: DataMember(Order = 1)] Guid Id) : ICommand; }
