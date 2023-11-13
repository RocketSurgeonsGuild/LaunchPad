using Nuke.Common.CI.GitHubActions;
using Nuke.Common.CI.GitHubActions.Configuration;
using Rocket.Surgery.Nuke.ContinuousIntegration;
using Rocket.Surgery.Nuke.DotNetCore;
using Rocket.Surgery.Nuke.GithubActions;

#pragma warning disable CA1050

[GitHubActionsSteps(
    "ci-ignore",
//    GitHubActionsImage.WindowsLatest,
    GitHubActionsImage.UbuntuLatest,
    AutoGenerate = false,
    On = new[] { RocketSurgeonGitHubActionsTrigger.Push },
    OnPushTags = new[] { "v*" },
    OnPushBranches = new[] { "master", "main", "next" },
    OnPullRequestBranches = new[] { "master", "main", "next" },
    Enhancements = new[] { nameof(CiIgnoreMiddleware) }
)]
[GitHubActionsSteps(
    "ci",
//    GitHubActionsImage.MacOsLatest,
//    GitHubActionsImage.WindowsLatest,
    GitHubActionsImage.UbuntuLatest,
    AutoGenerate = false,
    On = new[] { RocketSurgeonGitHubActionsTrigger.Push },
    OnPushTags = new[] { "v*" },
    OnPushBranches = new[] { "master", "main", "next" },
    OnPullRequestBranches = new[] { "master", "main", "next" },
    InvokedTargets = new[] { nameof(Default) },
    NonEntryTargets = new[]
    {
        nameof(ICIEnvironment.CIEnvironment),
        nameof(ITriggerCodeCoverageReports.TriggerCodeCoverageReports),
        nameof(ITriggerCodeCoverageReports.GenerateCodeCoverageReportCobertura),
        nameof(IGenerateCodeCoverageBadges.GenerateCodeCoverageBadges),
        nameof(IGenerateCodeCoverageReport.GenerateCodeCoverageReport),
        nameof(IGenerateCodeCoverageSummary.GenerateCodeCoverageSummary),
        nameof(Default)
    },
    ExcludedTargets = new[] { nameof(ICanClean.Clean), nameof(ICanRestoreWithDotNetCore.DotnetToolRestore) },
    Enhancements = new[] { nameof(CiMiddleware) }
)]
[PrintBuildVersion]
[PrintCIEnvironment]
[UploadLogs]
[TitleEvents]
[ContinuousIntegrationConventions]
public partial class Pipeline
{
    public static RocketSurgeonGitHubActionsConfiguration CiIgnoreMiddleware(RocketSurgeonGitHubActionsConfiguration configuration)
    {
        ( (RocketSurgeonsGithubActionsJob)configuration.Jobs[0] ).Steps = new List<GitHubActionsStep>
        {
            new RunStep("N/A")
            {
                Run = "echo \"No build required\""
            }
        };

        return configuration.IncludeRepositoryConfigurationFiles();
    }

    public static RocketSurgeonGitHubActionsConfiguration CiMiddleware(RocketSurgeonGitHubActionsConfiguration configuration)
    {
        var job = configuration
                 .ExcludeRepositoryConfigurationFiles()
                 .AddNugetPublish()
                 .Jobs.OfType<RocketSurgeonsGithubActionsJob>()
                 .First(z => z.Name.Equals("build", StringComparison.OrdinalIgnoreCase));
        job
           .UseDotNetSdks("6.0", "7.0")
           .AddNuGetCache()
            // .ConfigureForGitVersion()
           .ConfigureStep<CheckoutStep>(step => step.FetchDepth = 0)
           .PublishLogs<Pipeline>()
           .FailFast = false;

        job.Steps.Insert(
            GetCheckStepIndex(job)+1, new RunStep("Create branch for tag (gitversion)")
            {
                If = "(github.ref_type == 'tag')",
                Run = "git checkout -b ci/${{ github.ref }}"
            }
        );

        return configuration;
    }

    private static int GetCheckStepIndex(RocketSurgeonsGithubActionsJob job)
    {
        var checkoutStep = job.Steps.OfType<CheckoutStep>().SingleOrDefault();
        return checkoutStep is null ? 1 : job.Steps.IndexOf(checkoutStep);
    }
}
