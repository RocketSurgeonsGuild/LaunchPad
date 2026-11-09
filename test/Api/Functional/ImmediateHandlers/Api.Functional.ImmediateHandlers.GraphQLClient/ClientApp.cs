using Api.Functional.ImmediateHandlers.Contracts;
using Immediate.Handlers.Shared;
using Microsoft.Extensions.DependencyInjection;
namespace Api.Functional.ImmediateHandlers.GraphQLClient;

public static class ClientApp
{
    /// <summary>The client application's services: its mediator plus the generated GraphQL client, sending through <paramref name="handler" />.</summary>
    public static ServiceProvider Create(HttpMessageHandler handler)
    {
        var services = new ServiceCollection();

        services.AddRocketsGraphQLClient()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri("http://localhost/graphql"))
                .ConfigurePrimaryHttpMessageHandler(() => handler)
                .SetHandlerLifetime(Timeout.InfiniteTimeSpan);
        services.AddTransient<IRocketsDriver, Driver>();
        return services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
    }
}

internal sealed class Driver(IServiceProvider services) : IRocketsDriver
{
    // Immediate.Handlers has no mediator: a caller injects the (scoped) handler, which is what mediator.Send is here.
    private async Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
        where TRequest : notnull
    {
        await using var scope = services.CreateAsyncScope();
        return await scope.ServiceProvider.GetRequiredService<IHandler<TRequest, TResponse>>().HandleAsync(request, cancellationToken);
    }

    public async Task<Guid> CreateRocket(string name, CancellationToken cancellationToken) =>
        ( await Send<CreateRocket.Request, CreateRocket.Response>(new(name), cancellationToken) ).Id;

    // [ApiOperation(Response = typeof(RocketModel))] can't say "nullable", so a missing rocket is a NotFound RpcException on the client.
    // Queries aren't generated for GraphQL; the tests read through the server's HTTP endpoints.
    public Task<Rocket?> GetRocket(Guid id, CancellationToken cancellationToken) => throw new NotSupportedException();

    public IAsyncEnumerable<Rocket> ListRockets(string? name, CancellationToken cancellationToken) => throw new NotSupportedException();

    public async Task<Rocket> EditRocket(Guid id, string name, CancellationToken cancellationToken) =>
        await Send<EditRocket.Request, RocketModel>(new(id, name), cancellationToken) is var rocket ? new(rocket.Id, rocket.Name) : throw new InvalidOperationException();

    public async Task DeleteRocket(Guid id, CancellationToken cancellationToken) => await Send<DeleteRocket.Request, bool>(new(id), cancellationToken);
}
