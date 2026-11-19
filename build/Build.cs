#!/usr/bin/env dotnet
#:sdk Joinery.Dovetail.Sdk
#:package Sourcy.Git
#:package Sourcy.DotNet
#:package Rocket.Surgery.ModularPipelines.Extensions
#:include ./Samples.cs
#:property JsonSerializerIsReflectionEnabledByDefault=true
#:property ExperimentalFileBasedProgramEnableTransitiveDirectives=true

using Build;
using ModularPipelines;

// Joinery.Dovetail.ModularPipelines and Specular are direct references because their generators write ConfigureDovetail() and the
// module scan into this build host; they don't flow through Rocket.Surgery.ModularPipelines.Extensions. Their versions are the
// repository's (Directory.Packages.props). Dovetail reads the settings (appsettings.yaml) from this file's folder.
var builder = Pipeline.CreateBuilder(args);
// Registered before Dovetail's joints run: Joinery.Dovetail.OpenTelemetry (part of the Dovetail SDK) calls AddOpenTelemetry(), which
// TryAdds an IConfiguration that reads only the environment, ahead of the one ModularPipelines registers, so the modules would never
// see build/appsettings.yaml (no solution: "Not a solution file: ").
builder.Services.AddSingleton<Microsoft.Extensions.Configuration.IConfiguration>(builder.Configuration);
await using var pipeline = await builder.ConfigureDovetail();
await pipeline.RunAsync();
