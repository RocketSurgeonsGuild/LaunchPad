using System.Reflection;
using Hellang.Middleware.ProblemDetails;
using Microsoft.OpenApi.Models;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Rocket.Surgery.LaunchPad.AspNetCore.AppMetrics;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Sample.Classic.Restful;

/// <summary>
///     Startup interop (here for testing only or for 3.1 support)
/// </summary>
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
        app.UseProblemDetails();
        app.UseHttpsRedirection();

        app.UseLaunchPadRequestLogging();
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

                endpoints.MapSwagger();
                endpoints.MapAppMetrics();
            }
        );
    }
}
