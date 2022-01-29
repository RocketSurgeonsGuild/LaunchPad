using Microsoft.OpenApi.Models;
using Rocket.Surgery.LaunchPad.AspNetCore;

y.LaunchPad.AspNetCore;
using Rocket.Surgery.LaunchPad.AspNetCore.AppMetrics;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Sample.Restful
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddControllersAsServices();
            services
               .Configure<SwaggerGenOptions>(
                    c => c.SwaggerDoc(
                        "v1",
                        new OpenApiInfo
                        {
                            Version = typeof(Startup).GetCustomAttribute<AssemblyVersionAttribute>()?.Version
                                   ?? typeof(Startup).GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "0.1.0",
                            Title = "Test Application",
                        }
                    )
                );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();

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

            app
               .UseSwaggerUI()
               .UseReDoc();


            app.UseAuthorization();

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllers();

                    // Should this move into an extension method?
                    endpoints.MapSwagger();
                    endpoints.MapAppMetrics();
                }
            );
        }
    }
}
