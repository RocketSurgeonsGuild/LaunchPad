namespace Api.Functional;

public sealed record Rocket(Guid Id, string Name);

/// <summary>
///     What a test does with a client: each call goes through the client application's own mediator (<c>mediator.Send(...)</c>), which the
///     generated client handlers forward over HTTP to the server.
/// </summary>
public interface IRocketsDriver
{
    Task<Guid> CreateRocket(string name, CancellationToken cancellationToken);
    Task<Rocket?> GetRocket(Guid id, CancellationToken cancellationToken);
    IAsyncEnumerable<Rocket> ListRockets(string? name, CancellationToken cancellationToken);
    Task<Rocket> EditRocket(Guid id, string name, CancellationToken cancellationToken);
    Task DeleteRocket(Guid id, CancellationToken cancellationToken);
}
