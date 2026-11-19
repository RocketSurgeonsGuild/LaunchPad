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
await using var pipeline = await builder.ConfigureDovetail();
await pipeline.RunAsync();
