using System.Runtime.Loader;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Sample.Grpc;
using Sample.Grpc.Services;

var builder = await WebApplication.CreateBuilder(args)
                                  .LaunchWith(RocketBooster.For(Imports.GetConventions), b => b.Set(AssemblyLoadContext.Default));

var app = builder.Build();

app.UseLaunchPadRequestLogging();

app.UseRouting();

app.MapGrpcService<LaunchRecordsService>();
app.MapGrpcService<RocketsService>();

app.MapGet(
    "/",
    async context => await context.Response.WriteAsync(
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909"
    )
);

await app.RunAsync();

public partial class Program;
