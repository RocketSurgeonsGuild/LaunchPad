using System.Reflection;
using System.Runtime.Loader;
using Hellang.Middleware.ProblemDetails;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Sample.Classic.Restful;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication
   .CreateBuilder(args);
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
var app = await builder.LaunchWith(RocketBooster.For(Imports.Instance), b => b.Set(AssemblyLoadContext.Default));

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

public partial class Program;