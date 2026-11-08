using Api.Functional.ChannelMediator.Contracts;
using ChannelMediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Api;
namespace Api.Functional.ChannelMediator.Server;

public static class ServerApp
{
    /// <summary>Starts the server with the generated Minimal API endpoints, the generated controllers on the same routes, or the generated gRPC service.</summary>
    public static async Task<WebApplication> StartAsync(bool controllers = false, bool grpc = false)
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddSingleton<RocketStore>();
        builder.Services.AddProblemDetails();
        builder.Services.AddChannelMediator(_ => { }, typeof(Handlers).Assembly);
        if (controllers) builder.Services.AddControllers().AddApplicationPart(typeof(ServerApp).Assembly);
        if (grpc) ProtoBuf.Grpc.Server.ServicesExtensions.AddCodeFirstGrpc(builder.Services);
        var app = builder.Build();
        if (grpc) app.MapRocketsService();
        else if (controllers) app.MapControllers();
        else app.MapRocketsEndpoints();
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
