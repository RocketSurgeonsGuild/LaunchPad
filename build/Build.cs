#!/usr/bin/env dotnet
#:package Microsoft.VisualStudio.SolutionPersistence
#:package Sourcy.Git
#:package Sourcy.DotNet
#:package Rocket.Surgery.ModularPipelines.Extensions
#:package Joinery.Dovetail.ModularPipelines
#:package Specular
#:include ./Samples.cs
#:property JsonSerializerIsReflectionEnabledByDefault=true
#:property ExperimentalFileBasedProgramEnableTransitiveDirectives=true

using Build;
using ModularPipelines;

// Joinery.Dovetail.ModularPipelines and Specular are direct references because their generators write ConfigureDovetail() and the
// module scan into this build host; they don't flow through Rocket.Surgery.ModularPipelines.Extensions. Dovetail reads the settings
// (appsettings.yaml) from this file's folder.
await using var pipeline = await Pipeline.CreateBuilder(args).ConfigureDovetail();
await pipeline.RunAsync();
