using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

using Sample.Sdk.Console;

// ConfigureDovetail applies the joints the SDK brought in, LaunchPad.Hosting's health checks among them.
using var host = await Host.CreateApplicationBuilder(args).ConfigureDovetail();
var report = await host.Services.GetRequiredService<HealthCheckService>().CheckHealthAsync();
System.Console.WriteLine($"Health: {report.Status}");
