
#:package Microsoft.VisualStudio.SolutionPersistence
#:package Sourcy.Git
#:package Sourcy.DotNet
#:package Rocket.Surgery.ModularPipelines.Extensions
#:property ImportConventions=true
#:property JsonSerializerIsReflectionEnabledByDefault=true

using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using ModularPipelines.Attributes;
using ModularPipelines.Configuration;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.FileSystem;
using ModularPipelines.Models;
using ModularPipelines.Modules;
using ModularPipelines.Options;
using Rocket.Surgery.ModularPipelines.Extensions;
using Rocket.Surgery.ModularPipelines.Extensions.Modules;
using File = ModularPipelines.FileSystem.File;

[DependsOn<BuildSolution>]
// T008: shared AOT-publish helper. Each demonstration host publishes its scoped, in-csproj
// zero-warning property set (D4-pinned: PublishAot + TrimmerSingleWarn=false + TreatWarningsAsErrors)
// for net10.0 × current RID. A failed publish (or any promoted IL2xxx/IL3xxx) throws ⇒ the named
// module FAILs the build; an unsatisfied toolchain SKIPs-with-reason (aot-publish-step contract).
public class AotPublishModule() : Module<ImmutableList<CommandResult>>
{

    private readonly IReadOnlyCollection<FileInfo> projects = [
        Sourcy.DotNet.Projects.Sample_Command,
        Sourcy.DotNet.Projects.Sample_Minimal
    ];
    protected override ModuleConfiguration Configure() => ModuleConfiguration.Create().WithRetryCount(3).Build();
    protected override async Task<ImmutableList<CommandResult>> ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var rid = RuntimeInformation.RuntimeIdentifier;

        var results = await Task.WhenAll(projects.Select(project =>
        {
            var projectPath = new File(project.FullName);
            return context.SubModule(projectPath.NameWithoutExtension, async () =>
            {

                var hostname = projectPath.NameWithoutExtension;
                context.Logger.LogInformation("AOT publish [{Host}] × {Rid} (zero-warning policy)", hostname, rid);

                var result = await context.DotNet().Publish(
                    new DotNetPublishOptions
                    {
                        ProjectSolution = projectPath,
                        Configuration = "Release",
                        Runtime = rid,
                    },
                    new CommandExecutionOptions(),
                    cancellationToken);

                context.Logger.LogInformation("AOT publish [{Host}] PASS — zero trim/AOT warnings.", hostname);
                return result;
            });
        }));
        return [.. results];
    }
}
