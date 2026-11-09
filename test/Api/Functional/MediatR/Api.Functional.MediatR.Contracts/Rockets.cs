using System.Runtime.Serialization;
using MediatR;
using Rocket.Surgery.LaunchPad.Api;
namespace Api.Functional.MediatR.Contracts;

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

// A union response (C# 15's lowering of `union LaunchRocketResult(RocketModel, RocketNotFound, NotReady)`), served only by the Launches* hosts:
// the value case answers 200, RocketNotFound 404 from its name, NotReady 422 from [ApiError].
[DataContract] public sealed record RocketNotFound([property: DataMember(Order = 1)] Guid Id);

[ApiError(422, GrpcStatus = 9, GraphQLCode = "UNPROCESSABLE")]
[DataContract] public sealed record NotReady([property: DataMember(Order = 1)] string Reason);

[System.Runtime.CompilerServices.Union]
public readonly struct LaunchRocketResult : System.Runtime.CompilerServices.IUnion
{
    public LaunchRocketResult(RocketModel value) => Value = value;
    public LaunchRocketResult(RocketNotFound value) => Value = value;
    public LaunchRocketResult(NotReady value) => Value = value;
    public object? Value { get; }

    public static implicit operator LaunchRocketResult(RocketModel value) => new(value);
    public static implicit operator LaunchRocketResult(RocketNotFound value) => new(value);
    public static implicit operator LaunchRocketResult(NotReady value) => new(value);
}

public static class LaunchRocket { [DataContract] public sealed record Request([property: DataMember(Order = 1)] Guid Id) : IRequest<LaunchRocketResult>; }

// A stream of unions: launches every rocket in order, and ends with NotReady at the first grounded one.
public static class ListLaunches { [DataContract] public sealed record Request([property: DataMember(Order = 1)] string? Name) : IStreamRequest<LaunchRocketResult>; }
