using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Sample.Grpc.Services;
using Serilog;

namespace Sample.Grpc
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Should this move into an extension method?
            app.UseSerilogRequestLogging(
                x =>
                {
                    x.GetLevel = LaunchPadLogHelpers.DefaultGetLevel;
                    x.EnrichDiagnosticContext = LaunchPadLogHelpers.DefaultEnrichDiagnosticContext;
                }
            );
            app.UseMetricsAllMiddleware();

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
        }
    }
}