using System.Reflection;
using Hellang.Middleware.ProblemDetails;
using Microsoft.Extensions.DependencyModel;
using Microsoft.OpenApi.Models;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Sample.Classic.Restful;
using Swashbuckle.AspNetCore.SwaggerGen;


var builder = WebApplication.CreateBuilder(args);
await builder.LaunchWith(RocketBooster.For(Imports.GetConventions));
builder.Services.AddControllers().AddControllersAsServices();
builder.Services
       .Configure<SwaggerGenOptions>(
            c => c.SwaggerDoc(
                "v1",
                new()
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

app.UseLaunchPadRequestLogging();

app.UseRouting();

app
   .UseSwaggerUI()
   .UseReDoc();

app.UseAuthorization();

app.MapControllers();
app.MapSwagger();

await app.RunAsync();
