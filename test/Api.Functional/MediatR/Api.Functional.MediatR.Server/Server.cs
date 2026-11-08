using Api.Functional.MediatR.Contracts;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
namespace Api.Functional.MediatR.Server;

public static class ServerApp
{
    /// <summary>Starts the server with the generated Minimal API endpoints, the generated controllers on the same routes, or the generated gRPC service.</summary>
    public static async Task<WebApplication> StartAsync(bool controllers = false, bool grpc = false, bool graphql = false, bool versioned = false)
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddSingleton<RocketStore>();
        builder.Services.AddProblemDetails();
        builder.Services.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<Handlers>());
        if (controllers) builder.Services.AddControllers().AddApplicationPart(typeof(ServerApp).Assembly);
        if (grpc) ProtoBuf.Grpc.Server.ServicesExtensions.AddCodeFirstGrpc(builder.Services);
        // HotChocolate needs a query type; the generated mutations extend the Mutation type.
        if (graphql) builder.Services.AddGraphQLServer().AddQueryType(d => d.Name("Query").Field("ping").Resolve("pong")).AddMutationType().AddRocketsMutations();
        if (versioned)
        {
            var versioning = builder.Services.AddApiVersioning();
            if (controllers) versioning.AddMvc();
        }

        var app = builder.Build();
        // With controllers, VersionedRocketsController answers the same routes instead.
        if (versioned && !controllers) app.MapVersionedRocketsEndpoints();
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

public sealed class Handlers(RocketStore store)
    : IRequestHandler<GetRocket.Request, RocketModel?>, IStreamRequestHandler<ListRockets.Request, RocketModel>, IRequestHandler<CreateRocket.Request, CreateRocket.Response>,
      IRequestHandler<EditRocket.Request, RocketModel>, IRequestHandler<DeleteRocket.Request>
{
    public Task<RocketModel?> Handle(GetRocket.Request request, CancellationToken cancellationToken) =>
        Task.FromResult(store.Find(request.Id) is { } name ? new RocketModel(request.Id, name) : null);

    public async IAsyncEnumerable<RocketModel> Handle(ListRockets.Request request, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var (id, name) in store.All(request.Name)) { await Task.Yield(); yield return new(id, name); }
    }

    public Task<CreateRocket.Response> Handle(CreateRocket.Request request, CancellationToken cancellationToken) => Task.FromResult(new CreateRocket.Response(store.Add(request.Name)));

    public Task<RocketModel> Handle(EditRocket.Request request, CancellationToken cancellationToken)
    {
        store.Set(request.Id, request.Name);
        return Task.FromResult(new RocketModel(request.Id, request.Name));
    }

    public Task Handle(DeleteRocket.Request request, CancellationToken cancellationToken)
    {
        store.Remove(request.Id);
        return Task.CompletedTask;
    }
}

// Versions mapped with Asp.Versioning: GetRocket is only in 2.0, CreateRocket in the host's 1.0.
[OperationHost(Group = "versioned-rockets", Version = "1.0")]
[Operation<GetRocket.Request>(Version = "2.0")]
[Operation<CreateRocket.Request>]
public static partial class VersionedRocketsEndpoints;

// The same versions on a controller host, mapped with Asp.Versioning.Mvc.
[OperationHost(Group = "versioned-rockets", Version = "1.0")]
[Operation<GetRocket.Request>(Version = "2.0")]
[Operation<CreateRocket.Request>]
public partial class VersionedRocketsController;
