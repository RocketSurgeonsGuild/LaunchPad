using ZeroAlloc.Mediator;
namespace Api.Functional.ZeroAlloc.Contracts;

public sealed record RocketModel(Guid Id, string Name);

public static class GetRocket { public readonly record struct Request(Guid Id) : IRequest<RocketModel?>; }

public static class ListRockets { public readonly record struct Request(string? Name) : IStreamRequest<RocketModel>; }

public static class CreateRocket
{
    public readonly record struct Request(string Name) : IRequest<Response>;
    public sealed record Response(Guid Id);
}

public static class EditRocket { public readonly record struct Request(Guid Id, string Name) : IRequest<RocketModel>; }

public static class DeleteRocket { public readonly record struct Request(Guid Id) : IRequest; }
