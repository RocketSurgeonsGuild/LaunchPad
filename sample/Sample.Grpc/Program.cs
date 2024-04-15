using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Sample.Grpc;
using Sample.Grpc.Services;

var builder = await WebApplication.CreateBuilder(args)
                                  .LaunchWith(RocketBooster.For(Imports.GetConventions));

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
