using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.AspNetCore;

var builder = WebApplication
   .CreateBuilder(args);
builder.Services.AddControllers().AddControllersAsServices();
var app = await builder.ConfigureRocketSurgery();

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
