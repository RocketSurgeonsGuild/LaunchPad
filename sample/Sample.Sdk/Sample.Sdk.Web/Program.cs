using Rocket.Surgery.LaunchPad.AspNetCore;

using Sample.Sdk.Web;

// ConfigureDovetail applies the joints the SDK brought in; LaunchPad.AspNetCore's health checks joint registered the health checks mapped here.
var app = await WebApplication.CreateBuilder(args).ConfigureDovetail();
app.MapLaunchPadHealthChecks();
await app.RunAsync();
