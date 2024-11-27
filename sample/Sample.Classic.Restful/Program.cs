using System.Reflection;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Sample.Classic.Restful;

var builder = WebApplication
   .CreateBuilder(args);
builder.Services.AddControllers().AddControllersAsServices();
var app = await builder.LaunchWith(RocketBooster.For(Imports.Instance));

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseLaunchPadRequestLogging();

app.UseRouting();

app
   .UseSwaggerUI()
   .UseReDoc();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

public partial class Program;
