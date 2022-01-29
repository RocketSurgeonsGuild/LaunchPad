using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.MSBuild;
using Rocket.Surgery.Nuke.DotNetCore;

[PublicAPI]
[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
[PackageIcon("https://raw.githubusercontent.com/RocketSurgeonsGuild/graphics/master/png/social-square-thrust-rounded.png")]
[EnsureGitHooks(GitHook.PreCommit)]
[EnsureReadmeIsUpdated]
[DotNetVerbosityMapping]
[MSBuildVerbosityMapping]
[NuGetVerbosityMapping]
[ShutdownDotNetAfterServerBuild]
public partial class Solution : NukeBuild,
                                ICanRestoreWithDotNetCore,
                                ICanBuildWithDotNetCore,
                                ICanTestWithDotNetCore,
                                IHaveNuGetPackages,
                                IHaveDataCollector,
                                ICanClean,
                                ICanUpdateReadme,
                                IGenerateCodeCoverageReport,
                                IGenerateCodeCoverageSummary,
                                IGenerateCodeCoverageBadges,
                                IHaveConfiguration<Configuration>
{
    /// <summary>
    ///     Support plugins are available for:
    ///     - JetBrains ReSharper        https://nuke.build/resharper
    ///     - JetBrains Rider            https://nuke.build/rider
    ///     - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///     - Microsoft VSCode           https://nuke.build/vscode
    /// </summary>
    public static int Main()
    {
        return Execute<Solution>(x => x.Default);
    }

    [OptionalGitRepository] public GitRepository? GitRepository { get; }

    // public Target Pack => _ => _.Inherit<ICanPackWithDotNetCore>(x => x.CorePack)
    //    .DependsOn(Clean);

    /// <summary>
    ///     dotnet pack
    /// </summary>
    public Target Pack => _ => _
                              .Description("Packs all the NuGet packages.")
                              .DependsOn(Clean)
                              .After(Test)
                              .Executes(
                                   () =>
                                   {
                                       IHaveSolution selfSolution = this;
                                       IHaveNuGetPackages nuget = this;
                                       IHaveOutputLogs logs = this;
                                       return DotNetTasks.DotNetPack(
                                           s => s.SetProject(selfSolution.Solution)
                                                 .SetDefaultLoggers(logs.LogsDirectory / "pack.log")
                                                 .SetGitVersionEnvironment(GitVersion)
                                                 .SetConfiguration(Configuration)
                                                 .SetOutputDirectory(nuget.NuGetPackageDirectory)
                                       );
                                   }
                               );

    public Target BuildVersion => _ => _.Inherit<IHaveBuildVersion>(x => x.BuildVersion)
                                        .Before(Default)
                                        .Before(Clean);

    private Target Default => _ => _
                                  .DependsOn(Restore)
                                  .DependsOn(Build)
                                  .DependsOn(Test)
                                  .DependsOn(Pack);

    public Target Build => _ => _.Inherit<ICanBuildWithDotNetCore>(x => x.CoreBuild);

    [ComputedGitVersion] public GitVersion GitVersion { get; } = null!;

    public Target Clean => _ => _.Inherit<ICanClean>(x => x.Clean);
    public Target Restore => _ => _.Inherit<ICanRestoreWithDotNetCore>(x => x.CoreRestore);
    public Target Test => _ => _.Inherit<ICanTestWithDotNetCore>(x => x.CoreTest);

    [Parameter("Configuration to build")] public Configuration Configuration { get; } = IsLocalBuild ? Configuration.Debug : Configuration.Release;
}
