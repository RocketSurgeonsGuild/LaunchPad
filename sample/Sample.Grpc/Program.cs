using Rocket.Surgery.LaunchPad.AspNetCore;

using Sample.Grpc;
using Sample.Grpc.Services;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

// LaunchPad.Hosting still registers the Serilog request logging services with conventions that Dovetail does not run,
// until its Hosting joints replace them (RocketSurgeonsGuild/LaunchPad#3058)
builder.Services.AddSerilog(logger => logger.WriteTo.Console());

var app = await builder.ConfigureDovetail();

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

namespace Sample.Grpc
{
    public partial class Program;
}
