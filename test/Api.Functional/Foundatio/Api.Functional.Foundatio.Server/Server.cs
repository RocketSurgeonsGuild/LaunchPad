using Api.Functional.Foundatio.Contracts;
using Foundatio.Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Api;
namespace Api.Functional.Foundatio.Server;

public static class ServerApp
{
    public static async Task<WebApplication> StartAsync()
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddSingleton<RocketStore>();
        builder.Services.AddProblemDetails();
        // Scoped to this assembly: the test process also loads the client's handler module.
        builder.Services.AddMediator(b => b.AddAssembly(typeof(GetRocketHandler).Assembly));
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

// One class per request: Foundatio's generator names its wrappers after the message's simple name, which LaunchPad's nested X.Request types share.
public sealed class GetRocketHandler(RocketStore store)
{
    public RocketModel? Handle(GetRocket.Request request) => store.Find(request.Id) is { } name ? new RocketModel(request.Id, name) : null;
}

public sealed class ListRocketsHandler(RocketStore store)
{
    public async IAsyncEnumerable<RocketModel> Handle(ListRockets.Request request)
    {
        foreach (var (id, name) in store.All(request.Name)) { await Task.Yield(); yield return new(id, name); }
    }
}

public sealed class CreateRocketHandler(RocketStore store)
{
    public CreateRocket.Response Handle(CreateRocket.Request request) => new(store.Add(request.Name));
}

public sealed class EditRocketHandler(RocketStore store)
{
    public RocketModel Handle(EditRocket.Request request)
    {
        store.Set(request.Id, request.Name);
        return new(request.Id, request.Name);
    }
}

public sealed class DeleteRocketHandler(RocketStore store)
{
    public bool Handle(DeleteRocket.Request request) => store.Remove(request.Id);
}
