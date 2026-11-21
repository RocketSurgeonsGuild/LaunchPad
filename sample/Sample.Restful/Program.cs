using Rocket.Surgery.LaunchPad.AspNetCore;

using Sample.Restful;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddControllersAsServices();
builder.Services.AddSampleOpenApiSchemas();

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
app.MapLaunchPadHealthChecks();

app.MapControllers();

app.Run();

namespace Sample.Restful
{
    public partial class Program;
}
