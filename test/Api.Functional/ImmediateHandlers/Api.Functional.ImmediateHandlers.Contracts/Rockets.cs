using Rocket.Surgery.LaunchPad.Api;

namespace Api.Functional.ImmediateHandlers.Contracts;

// Immediate.Handlers requests implement nothing: the server reads the response from its [Handler], the client from [ApiOperation(Response)].
public sealed record RocketModel(Guid Id, string Name);

public static class GetRocket
{
    [ApiOperation(Response = typeof(RocketModel))]
    public sealed record Request(Guid Id);
}

public static class ListRockets
{
    [ApiOperation(Response = typeof(IAsyncEnumerable<RocketModel>))]
    public sealed record Request(string? Name);
}

public static class CreateRocket
{
    [ApiOperation(Response = typeof(Response))]
    public sealed record Request(string Name);
    public sealed record Response(Guid Id);
}

public static class EditRocket
{
    [ApiOperation(Response = typeof(RocketModel))]
    public sealed record Request(Guid Id, string Name);
}

public static class DeleteRocket
{
    [ApiOperation(Response = typeof(bool))]
    public sealed record Request(Guid Id);
}
