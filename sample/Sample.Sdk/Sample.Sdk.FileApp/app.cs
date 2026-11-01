#!/usr/bin/env dotnet
// A file-based app: no project file, run with `dotnet run Sample.Sdk.FileApp/app.cs`. The LaunchPad SDK reaches it through
// ../Directory.Build.props, as it reaches the projects next to it, and reads the #:package directives as the PackageReferences they are:
//   - Microsoft.Extensions.Hosting makes it a generic host, so it gets Joinery.Dovetail.Hosting, which generates ConfigureDovetail(), and
//     Rocket.Surgery.LaunchPad.Hosting, whose health check joint is written into the app because it references the health checks.
//   - NodaTime gets it Rocket.Surgery.LaunchPad.NodaTime, whose time joint registers the NodaTime clock.
#:package Microsoft.Extensions.Hosting@10.0.14
#:package Microsoft.Extensions.Diagnostics.HealthChecks@10.0.14
#:package NodaTime@3.3.7
// The namespace Dovetail generates ConfigureDovetail() in; a file-based app would otherwise take it from the file name.
#:property RootNamespace=Sample.Sdk.FileApp

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

using NodaTime;

using Sample.Sdk.FileApp;

using var host = await Host.CreateApplicationBuilder(args).ConfigureDovetail();
var report = await host.Services.GetRequiredService<HealthCheckService>().CheckHealthAsync();
Console.WriteLine($"Health: {report.Status}");
Console.WriteLine($"Clock: {host.Services.GetRequiredService<IClock>().GetType().Name}");
