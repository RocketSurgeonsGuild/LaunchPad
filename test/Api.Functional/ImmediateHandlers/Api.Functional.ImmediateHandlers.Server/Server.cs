using Api.Functional.ImmediateHandlers.Contracts;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Api;
namespace Api.Functional.ImmediateHandlers.Server;

public static class ServerApp
{
    /// <summary>Starts the server with the generated Minimal API endpoints, the generated controllers on the same routes, or the generated gRPC service.</summary>
    public static async Task<WebApplication> StartAsync(bool controllers = false, bool grpc = false, bool graphql = false)
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddSingleton<RocketStore>();
        builder.Services.AddProblemDetails();
        builder.Services.AddApiFunctionalImmediateHandlersServerHandlers();
        if (controllers) builder.Services.AddControllers().AddApplicationPart(typeof(ServerApp).Assembly);
        if (grpc) ProtoBuf.Grpc.Server.ServicesExtensions.AddCodeFirstGrpc(builder.Services);
        // HotChocolate needs a query type; the generated mutations extend the Mutation type.
        if (graphql) builder.Services.AddGraphQLServer().AddQueryType(d => d.Name("Query").Field("ping").Resolve("pong")).AddMutationType().AddRocketsMutations();
        var app = builder.Build();
        if (graphql)
        {
            app.MapGraphQL();
            app.MapRocketsEndpoints();
        }
        else if (grpc)
        {
            app.MapRocketsService();
        }
        else if (controllers)
        {
            app.MapControllers();
        }
        else
        {
            app.MapRocketsEndpoints();
        }

        await app.StartAsync();
        return app;
    }
}

[Operation<GetRocket.Request>]
[Operation<ListRockets.Request>]
[Operation<CreateRocket.Request>]
[Operation<EditRocket.Request>]
[Operation<DeleteRocket.Request>]
public static partial class RocketsEndpoints;

[Operation<GetRocket.Request>]
[Operation<ListRockets.Request>]
[Operation<CreateRocket.Request>]
[Operation<EditRocket.Request>]
[Operation<DeleteRocket.Request>]
public partial class RocketsController;

[Operation<GetRocket.Request>]
[Operation<ListRockets.Request>]
[Operation<CreateRocket.Request>]
[Operation<EditRocket.Request>]
[Operation<DeleteRocket.Request>]
public partial class RocketsService;

[Operation<CreateRocket.Request>]
[Operation<EditRocket.Request>]
[Operation<DeleteRocket.Request>]
public partial class RocketsMutations;

[Handler]
public static partial class GetRocketHandler
{
    private static ValueTask<RocketModel?> HandleAsync(GetRocket.Request request, RocketStore store, CancellationToken cancellationToken) =>
        new(store.Find(request.Id) is { } name ? new RocketModel(request.Id, name) : null);
}

[Handler]
public static partial class ListRocketsHandler
{
    private static ValueTask<IAsyncEnumerable<RocketModel>> HandleAsync(ListRockets.Request request, RocketStore store, CancellationToken cancellationToken) => new(List(request, store));

    private static async IAsyncEnumerable<RocketModel> List(ListRockets.Request request, RocketStore store)
    {
        foreach (var (id, name) in store.All(request.Name)) { await Task.Yield(); yield return new(id, name); }
    }
}

[Handler]
public static partial class CreateRocketHandler
{
    private static ValueTask<CreateRocket.Response> HandleAsync(CreateRocket.Request request, RocketStore store, CancellationToken cancellationToken) => new(new CreateRocket.Response(store.Add(request.Name)));
}

[Handler]
public static partial class EditRocketHandler
{
    private static ValueTask<RocketModel> HandleAsync(EditRocket.Request request, RocketStore store, CancellationToken cancellationToken)
    {
        store.Set(request.Id, request.Name);
        return new(new RocketModel(request.Id, request.Name));
    }
}

[Handler]
public static partial class DeleteRocketHandler
{
    private static ValueTask<bool> HandleAsync(DeleteRocket.Request request, RocketStore store, CancellationToken cancellationToken) => new(store.Remove(request.Id));
}
