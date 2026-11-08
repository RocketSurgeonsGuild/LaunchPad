using Api.Functional.Mediator.Contracts;
using Api.Functional.Mediator.GraphQLClientHost;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
namespace Api.Functional.Mediator.GraphQLClient;

public static class ClientApp
{
    /// <summary>The client application's services: its mediator plus the generated GraphQL client, sending through <paramref name="handler" />.</summary>
    public static ServiceProvider Create(HttpMessageHandler handler)
    {
        var services = new ServiceCollection();
        services.AddMediator();
        services.AddRocketsGraphQLClient()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri("http://localhost/graphql"))
                .ConfigurePrimaryHttpMessageHandler(() => handler)
                .SetHandlerLifetime(Timeout.InfiniteTimeSpan);
        services.AddTransient<IRocketsDriver, Driver>();
        return services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
    }
}

internal sealed class Driver(IMediator mediator) : IRocketsDriver
{
    public async Task<Guid> CreateRocket(string name, CancellationToken cancellationToken) => ( await mediator.Send(new CreateRocket.Request(name), cancellationToken) ).Id;

    // Queries aren't generated for GraphQL; the tests read through the server's HTTP endpoints.
    public Task<Rocket?> GetRocket(Guid id, CancellationToken cancellationToken) => throw new NotSupportedException();

    public IAsyncEnumerable<Rocket> ListRockets(string? name, CancellationToken cancellationToken) => throw new NotSupportedException();

    public async Task<Rocket> EditRocket(Guid id, string name, CancellationToken cancellationToken) =>
        await mediator.Send(new EditRocket.Request(id, name), cancellationToken) is var rocket ? new(rocket.Id, rocket.Name) : throw new InvalidOperationException();

    public async Task DeleteRocket(Guid id, CancellationToken cancellationToken) => await mediator.Send(new DeleteRocket.Request(id), cancellationToken);
}
