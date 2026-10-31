using Sample.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<BackgroundWorker>();

await ( await builder.ConfigureDovetail() ).RunAsync();
