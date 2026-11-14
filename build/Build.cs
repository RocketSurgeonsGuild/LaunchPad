#!/usr/bin/env dotnet
#:package Microsoft.VisualStudio.SolutionPersistence
#:package Rocket.Surgery.ModularPipelines.Extensions
#:package Joinery.Dovetail.ModularPipelines
#:package Specular
#:property JsonSerializerIsReflectionEnabledByDefault=true
#:property ExperimentalFileBasedProgramEnableTransitiveDirectives=true

using Build;
using ModularPipelines;

// Joinery.Dovetail.ModularPipelines and Specular are direct references because their generators write ConfigureDovetail() and the
// module scan into this build host; they don't flow through Rocket.Surgery.ModularPipelines.Extensions. Their versions are the
// repository's (Directory.Packages.props). Dovetail reads the settings (appsettings.yaml) from this file's folder.
await using var pipeline = await Pipeline.CreateBuilder(args).ConfigureDovetail();
await pipeline.RunAsync();
