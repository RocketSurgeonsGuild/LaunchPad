using Api.Functional.Mediator.Contracts;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Api;
namespace Api.Functional.Mediator.Server;

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

public sealed class Handlers(RocketStore store)
    : IQueryHandler<GetRocket.Request, RocketModel?>, IStreamQueryHandler<ListRockets.Request, RocketModel>, ICommandHandler<CreateRocket.Request, CreateRocket.Response>,
      ICommandHandler<EditRocket.Request, RocketModel>, ICommandHandler<DeleteRocket.Request>
{
    public ValueTask<RocketModel?> Handle(GetRocket.Request request, CancellationToken cancellationToken) =>
        new(store.Find(request.Id) is { } name ? new RocketModel(request.Id, name) : null);

    public async IAsyncEnumerable<RocketModel> Handle(ListRockets.Request request, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var (id, name) in store.All(request.Name)) { await Task.Yield(); yield return new(id, name); }
    }

    public ValueTask<CreateRocket.Response> Handle(CreateRocket.Request request, CancellationToken cancellationToken) => new(new CreateRocket.Response(store.Add(request.Name)));

    public ValueTask<RocketModel> Handle(EditRocket.Request request, CancellationToken cancellationToken)
    {
        store.Set(request.Id, request.Name);
        return new(new RocketModel(request.Id, request.Name));
    }

    public ValueTask<Unit> Handle(DeleteRocket.Request request, CancellationToken cancellationToken)
    {
        store.Remove(request.Id);
        return new(Unit.Value);
    }
}
