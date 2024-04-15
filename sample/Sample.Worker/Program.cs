using Microsoft.Extensions.DependencyModel;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;
using Sample.Worker;

var builder = await Host
                   .CreateApplicationBuilder(args)
                   .LaunchWith(RocketBooster.For(Imports.GetConventions));
builder.Services.AddHostedService<BackgroundWorker>();

await builder.RunAsync();
