using Rocket.Surgery.LaunchPad.AspNetCore;

using Sample.Classic.Restful;

var builder = WebApplication
   .CreateBuilder(args);
builder.Services.AddControllers().AddControllersAsServices();
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
            options.OAuthClientId("sample-web");
            options.OAuthUsePkce();
        }
    )
   .UseReDoc();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

namespace Sample.Classic.Restful
{
    public partial class Program;
}
