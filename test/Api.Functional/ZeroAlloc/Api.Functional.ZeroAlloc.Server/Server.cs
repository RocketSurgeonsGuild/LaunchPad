using Api.Functional.ZeroAlloc.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Api;
using ZeroAlloc.Mediator;
namespace Api.Functional.ZeroAlloc.Server;

public static class ServerApp
{
    /// <summary>Starts the server with the generated Minimal API endpoints, the generated controllers on the same routes, or the generated gRPC service.</summary>
    public static async Task<WebApplication> StartAsync(bool controllers = false, bool grpc = false, bool graphql = false)
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddSingleton<RocketStore>();
        builder.Services.AddProblemDetails();
        builder.Services.AddMediator();
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

public sealed class GetRocketHandler(RocketStore store) : IRequestHandler<GetRocket.Request, RocketModel?>
{
    public ValueTask<RocketModel?> Handle(GetRocket.Request request, CancellationToken cancellationToken) =>
        new(store.Find(request.Id) is { } name ? new RocketModel(request.Id, name) : null);
}

public sealed class ListRocketsHandler(RocketStore store) : IStreamRequestHandler<ListRockets.Request, RocketModel>
{
    public async IAsyncEnumerable<RocketModel> Handle(ListRockets.Request request, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var (id, name) in store.All(request.Name)) { await Task.Yield(); yield return new(id, name); }
    }
}

public sealed class CreateRocketHandler(RocketStore store) : IRequestHandler<CreateRocket.Request, CreateRocket.Response>
{
    public ValueTask<CreateRocket.Response> Handle(CreateRocket.Request request, CancellationToken cancellationToken) => new(new CreateRocket.Response(store.Add(request.Name)));
}

public sealed class EditRocketHandler(RocketStore store) : IRequestHandler<EditRocket.Request, RocketModel>
{
    public ValueTask<RocketModel> Handle(EditRocket.Request request, CancellationToken cancellationToken)
    {
        store.Set(request.Id, request.Name);
        return new(new RocketModel(request.Id, request.Name));
    }
}

public sealed class DeleteRocketHandler(RocketStore store) : IRequestHandler<DeleteRocket.Request, Unit>
{
    public ValueTask<Unit> Handle(DeleteRocket.Request request, CancellationToken cancellationToken)
    {
        store.Remove(request.Id);
        return new(Unit.Value);
    }
}
