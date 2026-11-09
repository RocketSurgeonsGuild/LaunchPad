using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Api.Aot.Contracts;
using Mediator;
using Rocket.Surgery.LaunchPad.Api;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Services.AddMediator();
builder.Services.AddSingleton(new ConcurrentDictionary<Guid, RocketModel>());
builder.Services.AddProblemDetails();
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.TypeInfoResolverChain.Insert(0, RocketsJsonContext.Default));
var app = builder.Build();
app.MapRocketsEndpoints();
app.Run();

[Operation<GetRocket.Request>]
[Operation<ListRockets.Request>]
[Operation<CreateRocket.Request>]
[Operation<EditRocket.Request>]
[Operation<DeleteRocket.Request>]
public static partial class RocketsEndpoints;

public sealed class Handlers(ConcurrentDictionary<Guid, RocketModel> store)
    : IQueryHandler<GetRocket.Request, RocketModel?>, IStreamQueryHandler<ListRockets.Request, RocketModel>, ICommandHandler<CreateRocket.Request, CreateRocket.Response>,
      ICommandHandler<EditRocket.Request, RocketModel>, ICommandHandler<DeleteRocket.Request>
{
    public ValueTask<RocketModel?> Handle(GetRocket.Request request, CancellationToken ct) => new(store.TryGetValue(request.Id, out var r) ? r : null);
    public async IAsyncEnumerable<RocketModel> Handle(ListRockets.Request request, [EnumeratorCancellation] CancellationToken ct)
    {
        foreach (var r in store.Values.Where(r => request.Name is null || r.Name == request.Name)) { await Task.Yield(); yield return r; }
    }
    public ValueTask<CreateRocket.Response> Handle(CreateRocket.Request request, CancellationToken ct) { var id = Guid.NewGuid(); store[id] = new(id, request.Name); return new(new CreateRocket.Response(id)); }
    public ValueTask<RocketModel> Handle(EditRocket.Request request, CancellationToken ct) => new(store[request.Id] = new(request.Id, request.Name));
    public ValueTask<Unit> Handle(DeleteRocket.Request request, CancellationToken ct) { store.TryRemove(request.Id, out _); return new(Unit.Value); }
}
