using Api.Functional.Mediator.ClientHost;
using Api.Functional.Mediator.Contracts;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
namespace Api.Functional.Mediator.Client;

public static class ClientApp
{
    /// <summary>The client application's services: its mediator plus the generated client, sending through <paramref name="handler" />.</summary>
    public static ServiceProvider Create(HttpMessageHandler handler)
    {
        var services = new ServiceCollection();
        services.AddMediator();
        services.AddRocketsClient()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri("http://localhost/"))
                .ConfigurePrimaryHttpMessageHandler(() => handler)
                .SetHandlerLifetime(Timeout.InfiniteTimeSpan);
        services.AddTransient<IRocketsDriver, Driver>();
        return services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
    }
}

internal sealed class Driver(IMediator mediator) : IRocketsDriver
{
    public async Task<Guid> CreateRocket(string name, CancellationToken cancellationToken) => ( await mediator.Send(new CreateRocket.Request(name), cancellationToken) ).Id;

    public async Task<Rocket?> GetRocket(Guid id, CancellationToken cancellationToken) =>
        await mediator.Send(new GetRocket.Request(id), cancellationToken) is { } rocket ? new(rocket.Id, rocket.Name) : null;

    public async IAsyncEnumerable<Rocket> ListRockets(string? name, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var rocket in mediator.CreateStream(new ListRockets.Request(name), cancellationToken)) yield return new(rocket.Id, rocket.Name);
    }

    public async Task<Rocket> EditRocket(Guid id, string name, CancellationToken cancellationToken) =>
        await mediator.Send(new EditRocket.Request(id, name), cancellationToken) is var rocket ? new(rocket.Id, rocket.Name) : throw new InvalidOperationException();

    public async Task DeleteRocket(Guid id, CancellationToken cancellationToken) => await mediator.Send(new DeleteRocket.Request(id), cancellationToken);
}
