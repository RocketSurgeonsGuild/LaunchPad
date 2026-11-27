using Rocket.Surgery.LaunchPad.AspNetCore;

using Sample.Minimal;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddControllersAsServices();
builder.Services.AddSampleOpenApiSchemas();
builder.Services.AddSampleAuthentication(builder.Configuration);

var app = await builder.ConfigureDovetail();
app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseLaunchPadRequestLogging();

app.UseRouting();
app.MapOpenApi();

app
   // Authorize signs in with the samples' public client (authorization code with PKCE) for the scopes the operations require
   .UseSwaggerUI(
        options =>
        {
            // the document ASP.NET Core serves (MapOpenApi), not Swashbuckle's own default address
            options.SwaggerEndpoint("/openapi/v1.json", "v1");
            options.OAuthClientId("sample-web");
            options.OAuthUsePkce();
        }
    )
   .UseReDoc();
app.MapLaunchRecordsEndpoints();
app.MapRocketsEndpoints();

app.UseAuthentication();
app.UseAuthorization();
app.MapLaunchPadHealthChecks();

app.Run();

namespace Sample.Minimal
{
    public partial class Program;
}
