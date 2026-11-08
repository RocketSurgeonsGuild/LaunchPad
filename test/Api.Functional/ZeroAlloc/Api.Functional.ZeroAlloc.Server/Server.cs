using Api.Functional.ZeroAlloc.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Api;
using ZeroAlloc.Mediator;
namespace Api.Functional.ZeroAlloc.Server;

public static class ServerApp
{
    public static async Task<WebApplication> StartAsync()
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddSingleton<RocketStore>();
        builder.Services.AddProblemDetails();
        builder.Services.AddMediator();
        var app = builder.Build();
        app.MapRocketsEndpoints();
        await app.StartAsync();
        return app;
    }
}

[Operation<GetRocket.Request>]
[Operation<ListRockets.Request>]
[CreateOperation<CreateRocket.Request, GetRocket.Request>]
[Operation<EditRocket.Request>]
[Operation<DeleteRocket.Request>]
public static partial class RocketsEndpoints;

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
