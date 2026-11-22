using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using Sample.BlazorWasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("app");
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new(builder.HostEnvironment.BaseAddress) });

// Sample.AppHost's Blazor gateway gives the client the address of Sample.Minimal (services__minimal__https__0, a route of the gateway that
// it proxies to the sample) as a runtime environment variable (wwwroot/Sample.BlazorWasm.lib.module.js). The configuration reads them, and
// service discovery resolves the client's address, https+http://minimal (wwwroot/appsettings.json), from it.
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddServiceDiscovery();
// the client reads and writes JSON through the generated context, not through reflection
builder.Services.AddSingleton(RocketsClientJson.Settings);

var host = await builder.ConfigureDovetail();
// in the browser console, and in the Aspire dashboard when Sample.AppHost serves the client
var logger = host.Services.GetRequiredService<ILogger<Program>>();
StartupLog.Started(logger, builder.HostEnvironment.BaseAddress);
await host.RunAsync();

namespace Sample.BlazorWasm
{
    internal static partial class StartupLog
    {
        [LoggerMessage(Level = LogLevel.Information, Message = "Sample.BlazorWasm started at {BaseAddress}")]
        public static partial void Started(ILogger logger, string baseAddress);
    }
}
