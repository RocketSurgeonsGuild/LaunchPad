using Hosting.Tests.Scaffolding;

using Sdk.Tests.Scaffolding;

namespace Sdk.Tests;

/// <summary>
///     A file-based app (a single <c>.cs</c> file run with <c>dotnet run app.cs</c>) gets the LaunchPad SDK from the
///     <c>Directory.Build.props</c> above it, as a project does, and its <c>#:package</c> directives are the package references the SDK
///     detects.
/// </summary>
/// <param name="targetFramework">The target framework of the throwaway app.</param>
[TargetFrameworks]
public class SdkFileBasedAppTests(string targetFramework)
{
    private static CancellationToken Cancellation => TestContext.Current!.Execution.CancellationToken;

    /// <summary>
    ///     Microsoft.Extensions.Hosting makes the app a generic host, which brings LaunchPad.Hosting and its health check joint, and NodaTime
    ///     brings LaunchPad.NodaTime and its clock: the app references neither LaunchPad package itself.
    /// </summary>
    [Test]
    public async Task Should_Apply_The_LaunchPad_Joints_To_A_File_Based_App()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        var folder = Directory.CreateDirectory(Path.Combine(tree.Directory, "fileapp")).FullName;
        await File.WriteAllTextAsync(
            Path.Combine(folder, "app.cs"),
            $$"""
            #:package Microsoft.Extensions.Hosting@{{PinnedVersion("Microsoft.Extensions.Hosting")}}
            #:package {{HostModelShape.HealthChecks}}@{{PinnedVersion(HostModelShape.HealthChecks)}}
            #:package NodaTime@{{PinnedVersion("NodaTime")}}
            #:property TargetFramework={{targetFramework}}
            #:property RootNamespace={{HostModelShape.RootNamespace}}
            using {{HostModelShape.RootNamespace}};

            using var host = await Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(args).ConfigureDovetail();
            System.Console.WriteLine($"HealthChecks:{host.Services.GetService(typeof(Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService)) is not null}");
            System.Console.WriteLine($"Clock:{host.Services.GetService(typeof(NodaTime.IClock))?.GetType().Name}");
            """,
            Cancellation
        );

        var output = await DotnetCli.Run("run fileapp/app.cs", tree.Directory, cancellationToken: Cancellation);

        output.ShouldContain("HealthChecks:True");
        output.ShouldContain("Clock:SystemClock");
    }

    private string PinnedVersion(string packageId) => Repository.PackageVersion(packageId, targetFramework);
}
