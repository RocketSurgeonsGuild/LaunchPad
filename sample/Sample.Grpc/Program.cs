using Rocket.Surgery.Hosting;
using Sample.Grpc;
using Sample.Grpc.Services;

var builder = await Host.CreateApplicationBuilder(args)
                        .LaunchWith(RocketBooster.For(Imports.GetConventions))

var app = builder.Build();

app.UseLaunchPadRequestLogging();

app.UseRouting();

app.UseEndpoints(
    endpoints =>
    {
        endpoints.MapGrpcService<LaunchRecordsService>();
        endpoints.MapGrpcService<RocketsService>();

        endpoints.MapGet(
            "/",
            async context => await context.Response.WriteAsync(
                "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909"
            )
        );
    }
);
