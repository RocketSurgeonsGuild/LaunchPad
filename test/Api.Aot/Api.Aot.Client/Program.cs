using Api.Aot.ClientLib;
using Api.Aot.Contracts;
using Mediator;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddMediator();
services.AddRocketsClient().ConfigureHttpClient(h => h.BaseAddress = new Uri(args.Length > 0 ? args[0] : "http://127.0.0.1:5098/"));
await using var provider = services.BuildServiceProvider();
var mediator = provider.GetRequiredService<IMediator>();
var created = await mediator.Send(new CreateRocket.Request("Falcon"));
Console.WriteLine($"get: {await mediator.Send(new GetRocket.Request(created.Id))}");
Console.WriteLine($"edit: {await mediator.Send(new EditRocket.Request(created.Id, "Falcon 9"))}");
await foreach (var r in mediator.CreateStream(new ListRockets.Request("Falcon 9"))) Console.WriteLine($"list: {r}");
await mediator.Send(new DeleteRocket.Request(created.Id));
Console.WriteLine($"after delete: {( await mediator.Send(new GetRocket.Request(created.Id)) )?.ToString() ?? "null (404)"}");
