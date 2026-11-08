using MediatR;
namespace Api.Functional.MediatR.Contracts;

public sealed record RocketModel(Guid Id, string Name);

public static class GetRocket { public sealed record Request(Guid Id) : IRequest<RocketModel?>; }

public static class ListRockets { public sealed record Request(string? Name) : IStreamRequest<RocketModel>; }

public static class CreateRocket
{
    public sealed record Request(string Name) : IRequest<Response>;
    public sealed record Response(Guid Id);
}

public static class EditRocket { public sealed record Request(Guid Id, string Name) : IRequest<RocketModel>; }

public static class DeleteRocket { public sealed record Request(Guid Id) : IRequest; }
