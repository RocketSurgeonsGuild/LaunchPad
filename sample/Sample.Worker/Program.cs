using Sample.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<BackgroundWorker>();
// The worker dispatches its operations itself, with no caller to authenticate: it is the caller, with the scopes it needs and no more.
builder.Services.AddScoped<ZeroAlloc.Authorization.ISecurityContext>(_ => WorkerIdentity.Instance);

await ( await builder.ConfigureDovetail() ).RunAsync();
