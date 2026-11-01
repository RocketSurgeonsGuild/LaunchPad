#!/usr/bin/env dotnet
// A file-based app: no project file, run with `dotnet run Sample.Sdk.FileApp/app.cs`. The #:sdk directive applies the LaunchPad SDK, which
// brings Microsoft.NET.Sdk (the default the directive replaces) through the Dovetail SDK it chains. As a file-based app it gets what a
// LaunchPad console application needs without a #:package directive:
//   - Microsoft.Extensions.Hosting and Joinery.Dovetail.Hosting, which generates ConfigureDovetail(), plus Joinery.Dovetail.Serilog and
//     Joinery.Dovetail.OpenTelemetry, from the Dovetail SDK the LaunchPad SDK chains;
//   - Rocket.Surgery.LaunchPad.Foundation, Rocket.Surgery.LaunchPad.Hosting and Rocket.Surgery.LaunchPad.NodaTime, whose time joint
//     registers the NodaTime clock.
// Its one #:package directive is the health checks, which LaunchPad.Hosting's health check joint is written into the app for.
#:sdk Rocket.Surgery.LaunchPad.Sdk@0.0.0-sample
#:package Microsoft.Extensions.Diagnostics.HealthChecks@10.0.14
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
