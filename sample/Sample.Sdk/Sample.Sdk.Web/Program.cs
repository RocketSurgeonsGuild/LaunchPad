using Sample.Sdk.Web;

// ConfigureDovetail applies the joints the SDK brought in; LaunchPad.Hosting's health check joint registered the health checks mapped here.
var app = await WebApplication.CreateBuilder(args).ConfigureDovetail();
app.MapHealthChecks("/health");
await app.RunAsync();
