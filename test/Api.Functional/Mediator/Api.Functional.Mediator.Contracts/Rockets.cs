using Mediator;
namespace Api.Functional.Mediator.Contracts;

public sealed record RocketModel(Guid Id, string Name);

public static class GetRocket { public sealed record Request(Guid Id) : IQuery<RocketModel?>; }

public static class ListRockets { public sealed record Request(string? Name) : IStreamQuery<RocketModel>; }

public static class CreateRocket
{
    public sealed record Request(string Name) : ICommand<Response>;
    public sealed record Response(Guid Id);
}

public static class EditRocket { public sealed record Request(Guid Id, string Name) : ICommand<RocketModel>; }

public static class DeleteRocket { public sealed record Request(Guid Id) : ICommand; }
