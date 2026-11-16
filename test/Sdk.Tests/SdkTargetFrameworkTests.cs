using Hosting.Tests.Scaffolding;

using Microsoft.Build.Utilities.ProjectCreation;

using Sdk.Tests.Scaffolding;

namespace Sdk.Tests;

/// <summary>
///     LaunchPad targets .NET 11 and later: a project on an older .NET that applies the SDK stops with an error naming the minimum
///     (src/Sdks/LaunchPad/Sdk/Rocket.Surgery.LaunchPad.TargetFramework.targets), and a .NET Standard library is left alone. The target frameworks the
///     repository supports pass in every other SDK test.
/// </summary>
public class SdkTargetFrameworkTests
{
    private static CancellationToken Cancellation => TestContext.Current!.Execution.CancellationToken;

    /// <summary>
    ///     An application on .NET 9 fails to restore with LPADSDK0001, before NuGet reports the LaunchPad packages as incompatible.
    /// </summary>
    /// <param name="host">The host model of the application.</param>
    [Test]
    [Arguments(HostModel.GenericHost)]
    [Arguments(HostModel.AspNetCore)]
    public async Task Should_Fail_With_The_Minimum_Target_Framework_Before_Net11(HostModel host)
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), "net9.0");
        tree.AddProject("app", host);

        var failure = await ( (Func<Task>)( () => DotnetCli.Run("restore app", tree.Directory, cancellationToken: Cancellation) ) ).Should().Throw<InvalidOperationException>();

        await failure!.Message.Should().Contain("error LPADSDK0001: LaunchPad requires net11.0 or later, and app targets net9.0.");
        await failure!.Message.Should().NotContain("NU1202");
    }

    /// <summary>
    ///     A project that targets .NET 8 next to a supported framework fails as well: the check runs per target framework.
    /// </summary>
    [Test]
    public async Task Should_Fail_For_An_Older_Framework_Among_Several()
    {
        var supported = Repository.TargetFrameworks[^1];
        using var tree = new SdkTree(await PackedSdk.Pack(), supported);
        tree.AddFile(
            "library/library.csproj",
            ProjectCreator
               .Templates.SdkCsproj(targetFramework: supported)
               .Property("TargetFramework", string.Empty)
               .Property("TargetFrameworks", $"net8.0;{supported}")
        );

        var failure = await ( (Func<Task>)( () => DotnetCli.Run("restore library", tree.Directory, cancellationToken: Cancellation) ) ).Should().Throw<InvalidOperationException>();

        await failure!.Message.Should().Contain("error LPADSDK0001: LaunchPad requires net11.0 or later, and library targets net8.0.");
    }

    /// <summary>
    ///     A .NET Standard library in a solution that applies the SDK to every project still restores: the SDK adds it nothing.
    /// </summary>
    [Test]
    public async Task Should_Leave_A_NetStandard_Library_Alone()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), "netstandard2.0");
        tree.AddProject("library", HostModel.None);

        var output = await DotnetCli.Run("restore library", tree.Directory, cancellationToken: Cancellation);

        await output.Should().NotContain("LPADSDK0001");
    }
}
