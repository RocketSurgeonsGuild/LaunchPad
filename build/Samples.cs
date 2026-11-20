using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using ModularPipelines;
using ModularPipelines.DotNet.Options;
using ModularPipelines.Context;
using ModularPipelines.Models;
using Rocket.Surgery.ModularPipelines.Extensions;
using Rocket.Surgery.ModularPipelines.Extensions.Modules;

namespace Build;

/// <summary>
///     Native AOT publish of the demonstration hosts. Each one publishes its in-project zero-warning property set (PublishAot,
///     TrimmerSingleWarn=false and TreatWarningsAsErrors) for each target framework on the current runtime identifier, as a sub module of its
///     own; a failed publish, or any trim or AOT warning it promotes, fails the build.
/// </summary>
[DependsOn<BuildSolution>]
public sealed class AotPublishModule(SolutionSettings settings) : Module<ImmutableList<CommandResult>>
{
    private static readonly IReadOnlyCollection<FileInfo> Projects =
    [
        Sourcy.DotNet.Projects.Sample_Command,
        Sourcy.DotNet.Projects.Sample_Minimal,
    ];

    // publish needs a framework: each one the samples target, LaunchPadTargetFrameworks in the repository's Directory.Build.props
    private static readonly IReadOnlyCollection<string> Frameworks = ReadTargetFrameworks(Projects.First().Directory!);

    protected override void Configure(ModuleConfigurationBuilder module) => module.WithRetry(3);

    [SuppressMessage("ModularPipelines", "MP0016", Justification = "Each sub module passes its own token on. MP0016 only recognizes ExecuteAsync's own.")]
    protected override async Task<ImmutableList<CommandResult>> ExecuteAsync(IModuleContext context, CancellationToken cancellationToken)
    {
        var rid = RuntimeInformation.RuntimeIdentifier;
        var results = await Task.WhenAll(
            Projects.SelectMany(project => Frameworks.Select(framework => (project, framework)))
                    .Select(item => context.RunSubModuleAsync(
                                $"{Path.GetFileNameWithoutExtension(item.project.Name)} {item.framework}",
                                token => PublishHostAsync(context, item.project, item.framework, rid, token),
                                cancellationToken
                            )
                     )
        );
        return [.. results];
    }

    private static string[] ReadTargetFrameworks(DirectoryInfo from)
    {
        var props = FindRootProps(from) ?? throw new InvalidOperationException($"No Directory.Build.props with LaunchPadTargetFrameworks above {from.FullName}");
        var frameworks = XDocument.Load(props).Descendants("LaunchPadTargetFrameworks").Single().Value;
        return frameworks.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    // The Directory.Build.props next to LaunchPad.slnx, in the folder or the nearest one above it.
    private static string? FindRootProps(DirectoryInfo from)
    {
        for (var directory = from; directory is not null; directory = directory.Parent)
        {
            var props = Path.Combine(directory.FullName, "Directory.Build.props");
            if (File.Exists(Path.Combine(directory.FullName, "LaunchPad.slnx")) && File.Exists(props))
            {
                return props;
            }
        }

        return null;
    }

    [SuppressMessage("ModularPipelines", "MP0016", Justification = "The sub module's token is passed on. MP0016 only recognizes ExecuteAsync's own.")]
    private async Task<CommandResult> PublishHostAsync(IModuleContext context, FileInfo project, string framework, string rid, CancellationToken cancellationToken)
    {
        var host = Path.GetFileNameWithoutExtension(project.Name);
        AotPublishLog.LogPublishing(context.Logger, host, framework, rid);
        var result = await context.Tools.DotNet.PublishAsync(
            new DotNetPublishOptions
            {
                ProjectSolution = project.FullName,
                Configuration = settings.Configuration,
                Framework = framework,
                Runtime = rid,
            },
            new(),
            cancellationToken
        );
        AotPublishLog.LogPublished(context.Logger, host, framework);
        return result;
    }
}

internal static partial class AotPublishLog
{
    [LoggerMessage(LogLevel.Information, "AOT publish [{Host}] {Framework} × {Rid} (zero-warning policy)")]
    public static partial void LogPublishing(ILogger logger, string host, string framework, string rid);

    [LoggerMessage(LogLevel.Information, "AOT publish [{Host}] {Framework} PASS: no trim or AOT warnings")]
    public static partial void LogPublished(ILogger logger, string host, string framework);
}
