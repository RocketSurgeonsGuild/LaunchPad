using Foundatio.Mediator;
using Rocket.Surgery.LaunchPad.Api;

namespace Api.Functional.Foundatio.Contracts;

public sealed record RocketModel(Guid Id, string Name);

public static class GetRocket { public sealed record Request(Guid Id) : IRequest<RocketModel?>; }

// Foundatio's markers are optional: a stream has none, so its response is named.
public static class ListRockets
{
    [ApiOperation(Response = typeof(IAsyncEnumerable<RocketModel>))]
    public sealed record Request(string? Name);
}

public static class CreateRocket
{
    public sealed record Request(string Name) : IRequest<Response>;
    public sealed record Response(Guid Id);
}

public static class EditRocket { public sealed record Request(Guid Id, string Name) : IRequest<RocketModel>; }

public static class DeleteRocket { public sealed record Request(Guid Id) : IRequest<bool>; }
