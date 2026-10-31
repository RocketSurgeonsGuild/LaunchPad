using Rocket.Surgery.LaunchPad.AspNetCore;

using Sample.Classic.Restful;

using Serilog;

var builder = WebApplication
   .CreateBuilder(args);
builder.Services.AddControllers().AddControllersAsServices();

// LaunchPad.Hosting still registers the Serilog request logging services with conventions that Dovetail does not run, until its Hosting joints replace them
// (RocketSurgeonsGuild/LaunchPad#3058)
builder.Services.AddSerilog(logger => logger.WriteTo.Console());

var app = await builder.ConfigureDovetail();

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseLaunchPadRequestLogging();

app.UseRouting();

app.MapOpenApi();
app
   .UseSwaggerUI()
   .UseReDoc();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

namespace Sample.Classic.Restful
{
    public partial class Program;
}
