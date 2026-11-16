using Microsoft.Build.Utilities.ProjectCreation;

using Sdk.Tests.Scaffolding;

namespace Sdk.Tests;

/// <summary>
///     The mirrors of the Microsoft SDKs: <c>Rocket.Surgery.LaunchPad.Sdk.Web</c> is <c>Microsoft.NET.Sdk.Web</c> with LaunchPad, so a
///     project swaps the one name for the other and changes nothing else. Each chains Dovetail's mirror of the same SDK
///     (<c>Joinery.Dovetail.Sdk.Web</c>) and then applies what <c>Rocket.Surgery.LaunchPad.Sdk</c> applies.
/// </summary>
/// <param name="targetFramework">The target framework of the throwaway app.</param>
[TargetFrameworks]
public class SdkMirrorTests(string targetFramework)
{
    private static CancellationToken Cancellation => TestContext.Current!.Execution.CancellationToken;

    /// <summary>
    ///     A project whose only SDK is a mirror gets what the Microsoft SDK with <c>Rocket.Surgery.LaunchPad.Sdk</c> next to it gets: the same
    ///     host model and the same packages. It restores and builds. The snapshot shows what that is for each SDK: a Razor class library and
    ///     an Aspire AppHost are not applications, so they get none of LaunchPad's hosting packages.
    /// </summary>
    /// <param name="name">The name of the mirror, after <c>Rocket.Surgery.LaunchPad.Sdk.</c>.</param>
    /// <param name="outputType">The output type, when the SDK does not imply it.</param>
    [Test]
    [Arguments("Web", null)]
    [Arguments("Worker", null)]
    // a Razor class library, which has no entry point
    [Arguments("Razor", "Library")]
    [Arguments("BlazorWebAssembly", null)]
    [Arguments("WebAssembly", null)]
    // a plain library: Microsoft.NET.Sdk.WindowsDesktop builds anywhere until the project targets Windows (UseWPF, UseWindowsForms)
    [Arguments("WindowsDesktop", "Library")]
    // Aspire.AppHost.Sdk comes from NuGet, at the version Joinery.Dovetail.Sdk.Aspire.AppHost was built against
    [Arguments("Aspire.AppHost", "Exe")]
    public async Task Should_Be_The_Microsoft_Sdk_With_LaunchPad(string name, string? outputType)
    {
        var packages = await PackedSdk.Pack();
        using var tree = new SdkTree(packages, targetFramework);
        AddStandalone(tree, "mirror", Project($"{PackedSdk.MirrorPackageId(name)}/{packages.Version}", outputType), outputType);
        AddStandalone(
            tree,
            "twosdks",
            Project(MicrosoftSdk(name), outputType).Sdk(PackedSdk.SdkPackageId, packages.Version),
            outputType
        );

        var mirror = await tree.Evaluate("mirror", Cancellation);
        var twoSdks = await tree.Evaluate("twosdks", Cancellation);
        await tree.Build("mirror", Cancellation);

        await string.Join(Environment.NewLine, mirror.Hosts).Should().BeEqualTo(string.Join(Environment.NewLine, twoSdks.Hosts));
        await string.Join(Environment.NewLine, mirror.PackageReferences).Should().BeEqualTo(string.Join(Environment.NewLine, twoSdks.PackageReferences));
        await Verify(mirror);
    }

    /// <summary>
    ///     A <c>Directory.Build.props</c> that applies <c>Rocket.Surgery.LaunchPad.Sdk</c> to every project (the tree's) is imported by the
    ///     Microsoft SDK the mirror brings first. The mirror then applies neither LaunchPad nor Joinery.Dovetail.Sdk a second time, and the
    ///     project gets what it gets on its own.
    /// </summary>
    [Test]
    public async Task Should_Not_Apply_LaunchPad_Twice_Under_A_Directory_Build_Props_That_Applies_It()
    {
        var packages = await PackedSdk.Pack();
        using var tree = new SdkTree(packages, targetFramework);
        tree.AddFile("web/web.csproj", Project($"{PackedSdk.MirrorPackageId("Web")}/{packages.Version}", null));
        await File.WriteAllTextAsync(Path.Combine(tree.Directory, "web", "Program.cs"), Program, Cancellation);

        var evaluation = await tree.Evaluate("web", Cancellation);
        var output = await tree.Build("web", Cancellation);

        // MSB4011: a file imported a second time, which is what applying an SDK twice would log
        await output.Should().NotContain("MSB4011");
        await Verify(evaluation).UseMethodName(WebSnapshot);
    }

    /// <summary>
    ///     A <c>Directory.Build.props</c> that applies Joinery.Dovetail.Sdk alone: Dovetail's mirror leaves it out, and the LaunchPad mirror
    ///     still adds LaunchPad's packages, which Dovetail's SDK, applied before LaunchPad's detection was registered, turns into references.
    /// </summary>
    [Test]
    public async Task Should_Add_LaunchPad_Under_A_Directory_Build_Props_That_Applies_The_Dovetail_Sdk()
    {
        var packages = await PackedSdk.Pack();
        using var tree = new SdkTree(packages, targetFramework);
        // the Directory.Build.props in the project's folder, which is the one MSBuild finds first
        tree
           .AddFile(
                "dovetail/Directory.Build.props",
                ProjectCreator.Create(sdk: $"Joinery.Dovetail.Sdk/{PackedSdk.DovetailVersion}").Property("NuGetAudit", "false")
            )
           .AddFile("dovetail/dovetail.csproj", Project($"{PackedSdk.MirrorPackageId("Web")}/{packages.Version}", null));
        await File.WriteAllTextAsync(Path.Combine(tree.Directory, "dovetail", "Program.cs"), Program, Cancellation);

        var evaluation = await tree.Evaluate("dovetail", Cancellation);
        var output = await tree.Build("dovetail", Cancellation);

        await output.Should().NotContain("MSB4011");
        await Verify(evaluation).UseMethodName(WebSnapshot);
    }

    /// <summary>
    ///     Every mirror ships the same files: its Sdk.props and Sdk.targets, the files every LaunchPad SDK shares and the one that applies them
    ///     after Dovetail's mirror. One snapshot for all of them, with the package id (the name of the nuspec) taken out.
    /// </summary>
    /// <param name="name">The name of the mirror.</param>
    [Test]
    [MethodDataSource(nameof(MirrorNames))]
    public async Task Should_Ship_Only_Msbuild_Files(string name)
    {
        var packages = await PackedSdk.Pack();

        await Verify(packages.Mirrors[name].Files.Select(z => z.Replace(PackedSdk.MirrorPackageId(name), "{PackageId}", StringComparison.Ordinal)))
           .IgnoreParametersForVerified();
    }

    /// <summary>
    ///     Each mirror chains Dovetail's mirror at the version the repository pins and adds the LaunchPad packages at its own version. One
    ///     snapshot for all of them, with the package id taken out.
    /// </summary>
    /// <param name="name">The name of the mirror.</param>
    [Test]
    [MethodDataSource(nameof(MirrorNames))]
    public async Task Should_Generate_The_Package_Versions_When_Packed(string name)
    {
        var packages = await PackedSdk.Pack();

        await Verify(packages.Scrub(packages.Mirrors[name].PackagesProps).Replace(PackedSdk.MirrorPackageId(name), "{PackageId}", StringComparison.Ordinal), "xml")
           .IgnoreParametersForVerified();
    }

    /// <summary>
    ///     The names of the mirrors.
    /// </summary>
    /// <returns>The names.</returns>
    public static IEnumerable<string> MirrorNames() => PackedSdk.MirrorNames;

    /// <summary>
    ///     The snapshot of the Web mirror on its own, which a Web mirror under a Directory.Build.props that applies an SDK shares: it gets the
    ///     same.
    /// </summary>
    private const string WebSnapshot = $"{nameof(Should_Be_The_Microsoft_Sdk_With_LaunchPad)}_name=Web_outputType=null";

    private const string Program = """System.Console.WriteLine("hello");""";

    /// <summary>
    ///     The Microsoft SDK a mirror stands for; Aspire.AppHost.Sdk comes from NuGet, at the version the repository pins.
    /// </summary>
    private string MicrosoftSdk(string name) =>
        name == "Aspire.AppHost" ? $"Aspire.AppHost.Sdk/{Repository.PackageVersion("Aspire.AppHost.Sdk", targetFramework)}" : $"Microsoft.NET.Sdk.{name}";

    private ProjectCreator Project(string sdk, string? outputType)
    {
        var project = ProjectCreator.Templates.SdkCsproj(sdk: sdk, targetFramework: targetFramework, outputType: outputType);
        // NETSDK1137: Microsoft.NET.Sdk covers Windows desktop projects since .NET 5. The mirror is for the projects that still name it.
        if (sdk.Contains("WindowsDesktop", StringComparison.Ordinal)) project.Property("NoWarn", "$(NoWarn);NETSDK1137");
        // A WebAssembly application references the WebAssembly host, as HostModel.WebAssembly does. Without it Joinery.Dovetail 0.7.0's
        // Joinery.Dovetail.Environment.targets fails the build (MSB4184, a template named ".cs.template"), with either SDK form.
        if (sdk.Contains("WebAssembly", StringComparison.Ordinal))
        {
            project.ItemPackageReference(WebAssemblyHost, Repository.PackageVersion(WebAssemblyHost, targetFramework));
        }

        return project;
    }

    private const string WebAssemblyHost = "Microsoft.AspNetCore.Components.WebAssembly";

    /// <summary>
    ///     A project in a folder of its own, with a <c>Directory.Build.props</c> of its own, so the tree's, which applies
    ///     <c>Rocket.Surgery.LaunchPad.Sdk</c> to every project below it, doesn't reach it.
    /// </summary>
    private static void AddStandalone(SdkTree tree, string name, ProjectCreator project, string? outputType)
    {
        tree
           .AddFile($"{name}/Directory.Build.props", ProjectCreator.Create().Property("NuGetAudit", "false"))
           .AddFile($"{name}/{name}.csproj", project);
        if (outputType != "Library") File.WriteAllText(Path.Combine(tree.Directory, name, "Program.cs"), Program);
    }
}
