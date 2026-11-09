using System.Reflection;
using Api.Functional.MediatR.Contracts;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Api;
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
        builder.Services.AddFixtureOpenApi();
        builder.Services.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<Handlers>());
        if (controllers)
        {
            var mvc = builder.Services.AddControllers().AddApplicationPart(typeof(ServerApp).Assembly);
            // VersionedRocketsController answers only with versioning on, so the other servers' documents (no versioned hosts) match this one's.
            if (!versioned) mvc.ConfigureApplicationPartManager(parts => parts.FeatureProviders.Add(new WithoutVersionedRockets()));
        }
        if (grpc) ProtoBuf.Grpc.Server.ServicesExtensions.AddCodeFirstGrpc(builder.Services);
        // HotChocolate needs a query type; the generated mutations extend the Mutation type.
        if (graphql) builder.Services.AddGraphQLServer().AddQueryType(d => d.Name("Query").Field("ping").Resolve("pong")).AddMutationType().AddRocketsMutations();
        if (versioned)
        {
            var versioning = builder.Services.AddApiVersioning();
            if (controllers) versioning.AddMvc();
        }

        var app = builder.Build();
        app.MapOpenApi();
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

internal sealed class WithoutVersionedRockets : Microsoft.AspNetCore.Mvc.ApplicationParts.IApplicationFeatureProvider<Microsoft.AspNetCore.Mvc.Controllers.ControllerFeature>
{
    public void PopulateFeature(IEnumerable<Microsoft.AspNetCore.Mvc.ApplicationParts.ApplicationPart> parts, Microsoft.AspNetCore.Mvc.Controllers.ControllerFeature feature) =>
        feature.Controllers.Remove(typeof(VersionedRocketsController).GetTypeInfo());
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

// Versions mapped with Asp.Versioning: GetRocket is only in 2.0, CreateRocket in the host's 1.0. Their own names keep the main hosts' endpoint
// names (and so the OpenAPI operation ids every mediator's server shares) free of a host prefix.
[OperationHost(Group = "versioned-rockets", Version = "1.0")]
[Operation<GetRocket.Request>(Version = "2.0", Name = "GetVersionedRocket", Route = "{id}/rocket")]
[Operation<CreateRocket.Request>(Name = "CreateVersionedRocket", Route = "rocket")]
public static partial class VersionedRocketsEndpoints;

// The same versions on a controller host, mapped with Asp.Versioning.Mvc.
[OperationHost(Group = "versioned-rockets", Version = "1.0")]
[Operation<GetRocket.Request>(Version = "2.0", Name = "GetVersionedRocket", Route = "{id}/rocket")]
[Operation<CreateRocket.Request>(Name = "CreateVersionedRocket", Route = "rocket")]
public partial class VersionedRocketsController;
