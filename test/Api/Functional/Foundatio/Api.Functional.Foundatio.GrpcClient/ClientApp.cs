using Api.Functional.Foundatio.Contracts;
using Api.Functional.Foundatio.GrpcClientHost;
using Foundatio.Mediator;
using Microsoft.Extensions.DependencyInjection;
namespace Api.Functional.Foundatio.GrpcClient;

public static class ClientApp
{
    /// <summary>The client application's services: its mediator plus the generated gRPC client, sending through <paramref name="handler" />.</summary>
    public static ServiceProvider Create(HttpMessageHandler handler)
    {
        var services = new ServiceCollection();
        services.AddMediator(b => b.AddAssembly(typeof(RocketsGrpcClient).Assembly));
        services.AddRocketsGrpcClient(options => options.Address = new Uri("http://localhost/"))
                .ConfigurePrimaryHttpMessageHandler(() => handler)
                .SetHandlerLifetime(Timeout.InfiniteTimeSpan);
        services.AddTransient<IRocketsDriver, Driver>();
        return services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
    }
}

internal sealed class Driver(IMediator mediator) : IRocketsDriver
{
    public async Task<Guid> CreateRocket(string name, CancellationToken cancellationToken) => ( await mediator.InvokeAsync(new CreateRocket.Request(name), cancellationToken) ).Id;

    public async Task<Rocket?> GetRocket(Guid id, CancellationToken cancellationToken) =>
        await mediator.InvokeAsync(new GetRocket.Request(id), cancellationToken) is { } rocket ? new(rocket.Id, rocket.Name) : null;

    public async IAsyncEnumerable<Rocket> ListRockets(string? name, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var rockets = await mediator.InvokeAsync<IAsyncEnumerable<RocketModel>>(new ListRockets.Request(name), cancellationToken);
        await foreach (var rocket in rockets.WithCancellation(cancellationToken)) yield return new(rocket.Id, rocket.Name);
    }

    public async Task<Rocket> EditRocket(Guid id, string name, CancellationToken cancellationToken) =>
        await mediator.InvokeAsync(new EditRocket.Request(id, name), cancellationToken) is var rocket ? new(rocket.Id, rocket.Name) : throw new InvalidOperationException();

    public async Task DeleteRocket(Guid id, CancellationToken cancellationToken) => await mediator.InvokeAsync(new DeleteRocket.Request(id), cancellationToken);
}
