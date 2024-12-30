using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Sample.Grpc.Services;

var app = await WebApplication
               .CreateBuilder(args)
               .ConfigureRocketSurgery();

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

[System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
public partial class Program
{
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private string DebuggerDisplay
    {
        get
        {
            return ToString();
        }
    }
}