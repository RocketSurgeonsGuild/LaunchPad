using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.MSBuild;
using Polly;
using Rocket.Surgery.Nuke.DotNetCore;
using Serilog;

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
public partial class Pipeline : NukeBuild,
                                ICanRestoreWithDotNetCore,
                                ICanBuildWithDotNetCore,
                                ICanTestWithDotNetCore,
                                IComprehendSamples,
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
        return Execute<Pipeline>(x => x.Default);
    }

    public static int FindFreePort()
    {
        var port = 0;
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            var localEP = new IPEndPoint(IPAddress.Any, 0);
            socket.Bind(localEP);
            localEP = (IPEndPoint)socket.LocalEndPoint;
            port = localEP.Port;
        }
        finally
        {
            socket.Close();
        }

        return port;
    }

    public Target Pack => _ => _.Inherit<ICanPackWithDotNetCore>(x => x.CorePack)
                                .DependsOn(Clean);

    public Target BuildVersion => _ => _.Inherit<IHaveBuildVersion>(x => x.BuildVersion)
                                        .Before(Default)
                                        .Before(Clean);

    [OptionalGitRepository] public GitRepository? GitRepository { get; }

    private Target Default => _ => _
                                  .DependsOn(Restore)
                                  .DependsOn(Build)
                                  .DependsOn(Test)
                                  .DependsOn(Pack);

    [Solution(GenerateProjects = true)] private Solution Solution { get; } = null!;

    public Target Build => _ => _.Inherit<ICanBuildWithDotNetCore>(x => x.CoreBuild);

    public Target Clean => _ => _.Inherit<ICanClean>(x => x.Clean);
    public Target Restore => _ => _.Inherit<ICanRestoreWithDotNetCore>(x => x.CoreRestore);
    Nuke.Common.ProjectModel.Solution IHaveSolution.Solution => Solution;
    [ComputedGitVersion] public GitVersion GitVersion { get; } = null!;
    public Target Test => _ => _.Inherit<ICanTestWithDotNetCore>(x => x.CoreTest);
    [Parameter("Configuration to build")] public Configuration Configuration { get; } = IsLocalBuild ? Configuration.Debug : Configuration.Release;
}
