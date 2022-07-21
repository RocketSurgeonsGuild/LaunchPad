using System.Reflection;
using Hellang.Middleware.ProblemDetails;
using Microsoft.OpenApi.Models;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Rocket.Surgery.Web.Hosting;
using Sample.Restful;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args)
                            .ConfigureRocketSurgery(Imports.GetConventions);

builder.Services.AddControllers().AddControllersAsServices();
builder.Services
       .Configure<SwaggerGenOptions>(
            c => c.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Version = typeof(Program).GetCustomAttribute<AssemblyVersionAttribute>()?.Version
                           ?? typeof(Program).GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "0.1.0",
                    Title = "Test Application",
                }
            )
        );

var app = builder.Build();
app.UseProblemDetails();
app.UseHttpsRedirection();

// Should this move into an extension method?
app.UseSerilogRequestLogging(
    x =>
    {
        x.GetLevel = LaunchPadLogHelpers.DefaultGetLevel;
        x.EnrichDiagnosticContext = LaunchPadLogHelpers.DefaultEnrichDiagnosticContext;
    }
);

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
    }
);
app.Run();

public partial class Program
{
}
