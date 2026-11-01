using Sample.Grpc.Services;

var builder = WebApplication.CreateBuilder(args);

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
