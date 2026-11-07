#!/usr/bin/env dotnet
#:package Microsoft.VisualStudio.SolutionPersistence
#:package Sourcy.Git
#:package Sourcy.DotNet
#:package Rocket.Surgery.ModularPipelines.Extensions
#:include ./Samples.cs
#:property ImportConventions=true
#:property JsonSerializerIsReflectionEnabledByDefault=true
#:property ExperimentalFileBasedProgramEnableTransitiveDirectives=true

using Build;
using ModularPipelines;
using ModularPipelines.Plugins;
using Rocket.Surgery.ModularPipelines.Extensions;
using Rocket.Surgery.ModularPipelines.Extensions.Modules;

var pipelineBuilder = Pipeline.CreateBuilder(args);
PluginRegistry.Register(new ConventionsPlugin(ConventionContextBuilder.Create(Imports.Instance)
.AddIfMissing(nameof(AssemblyInfo.Project.BuildScriptsRoot), AssemblyInfo.Project.BuildScriptsRoot)));
await pipelineBuilder.Build().RunAsync();
