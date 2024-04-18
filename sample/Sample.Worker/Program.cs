using Rocket.Surgery.Hosting;
using Sample.Worker;

var builder = Host
   .CreateApplicationBuilder(args);
builder.Services.AddHostedService<BackgroundWorker>();


await ( await builder.LaunchWith(RocketBooster.For(Imports.Instance)) ).RunAsync();