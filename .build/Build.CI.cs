using Nuke.Common.CI.GitHubActions;
using Nuke.Common.CI.GitHubActions.Configuration;
using Rocket.Surgery.Nuke.ContinuousIntegration;
using Rocket.Surgery.Nuke.GithubActions;

#pragma warning disable CA1050

[GitHubActionsSteps(
    "ci-ignore",
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
    GitHubActionsImage.UbuntuLatest,
    AutoGenerate = false,
    On = new[] { RocketSurgeonGitHubActionsTrigger.Push },
    OnPushTags = new[] { "v*" },
    OnPushBranches = new[] { "master", "main", "next" },
    OnPullRequestBranches = new[] { "master", "main", "next" },
    InvokedTargets = new[] { nameof(Default) },
    Enhancements = new[] { nameof(CiMiddleware) }
)]
[GitHubActionsLint(
    "lint",
    GitHubActionsImage.UbuntuLatest,
    AutoGenerate = false,
    OnPullRequestTargetBranches = new[] { "master", "main", "next" },
    Enhancements = new[] { nameof(LintStagedMiddleware) }
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
        ((RocketSurgeonsGithubActionsJob)configuration.Jobs[0]).Steps = new List<GitHubActionsStep>
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
           .UseDotNetSdks("8.0", "9.0")
           .ConfigureStep<CheckoutStep>(step => step.FetchDepth = 0)
           .PublishLogs<Pipeline>();

        return configuration;
    }

    public static RocketSurgeonGitHubActionsConfiguration LintStagedMiddleware(RocketSurgeonGitHubActionsConfiguration configuration)
    {
        configuration
           .Jobs.OfType<RocketSurgeonsGithubActionsJob>()
           .First(z => z.Name.Equals("Build", StringComparison.OrdinalIgnoreCase))
           .UseDotNetSdks("8.0", "9.0");

        return configuration;
    }
}
