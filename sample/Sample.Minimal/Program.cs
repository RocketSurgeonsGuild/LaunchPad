using Rocket.Surgery.LaunchPad.AspNetCore;

using Sample.Minimal;

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
app.MapLaunchRecordsEndpoints();
app.MapRocketsEndpoints();

app.UseAuthorization();
app.MapLaunchPadHealthChecks();

app.Run();

namespace Sample.Minimal
{
    public partial class Program;
}
