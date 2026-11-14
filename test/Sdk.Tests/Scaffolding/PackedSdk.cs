using System.IO.Compression;
using System.Xml.Linq;

using Hosting.Tests.Scaffolding;

namespace Sdk.Tests.Scaffolding;

/// <summary>
///     <c>Rocket.Surgery.LaunchPad.Sdk</c> and the packages it adds to an application, packed from the repository into a feed of their own,
///     once for all the tests. The version is unique to the run, so a copy an earlier run left in a packages folder cannot stand in for it.
/// </summary>
/// <param name="Feed">The folder holding the packages; a folder with .nupkg files is a NuGet feed.</param>
/// <param name="Version">The version of every package in the feed.</param>
/// <param name="SdkFiles">The files in the SDK package, without the package bookkeeping or the icon CI adds.</param>
/// <param name="PackagesProps">The text of the props file the SDK generates when it is packed.</param>
internal sealed record PackedSdk(string Feed, string Version, IReadOnlyList<string> SdkFiles, string PackagesProps)
{
    /// <summary>
    ///     The id of the SDK package.
    /// </summary>
    public const string SdkPackageId = "Rocket.Surgery.LaunchPad.Sdk";

    /// <summary>
    ///     The projects packed into the feed: the SDK, every package it can add to an application, and the LaunchPad packages
    ///     those depend on, so an application that restores them finds each in the feed.
    /// </summary>
    private static readonly string[] Projects =
    [
        Path.Combine("src", "Sdk", "Rocket.Surgery.LaunchPad.Sdk.csproj"),
        Path.Combine("src", "Hosting", "Rocket.Surgery.LaunchPad.Hosting.csproj"),
        Path.Combine("src", "Analyzers", "Rocket.Surgery.LaunchPad.Analyzers.csproj"),
        Path.Combine("src", "Primitives", "Rocket.Surgery.LaunchPad.Primitives.csproj"),
        Path.Combine("src", "Primitives", "Analyzers", "Rocket.Surgery.LaunchPad.Primitives.Analyzers.csproj"),
        Path.Combine("src", "Foundation", "Rocket.Surgery.LaunchPad.Foundation.csproj"),
        Path.Combine("src", "AspNetCore", "Rocket.Surgery.LaunchPad.AspNetCore.csproj"),
        Path.Combine("src", "NodaTime", "Rocket.Surgery.LaunchPad.NodaTime.csproj"),
        Path.Combine("src", "NodaTime.HotChocolate", "Rocket.Surgery.LaunchPad.NodaTime.HotChocolate.csproj"),
        Path.Combine("src", "NodaTime.StrawberryShake", "Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake.csproj"),
        Path.Combine("src", "Spatial", "Rocket.Surgery.LaunchPad.Spatial.csproj"),
        Path.Combine("src", "StrawberryShake", "Rocket.Surgery.LaunchPad.StrawberryShake.csproj"),
        Path.Combine("src", "StrawberryShake.Spatial", "Rocket.Surgery.LaunchPad.StrawberryShake.Spatial.csproj"),
        Path.Combine("src", "HotChocolate", "Rocket.Surgery.LaunchPad.HotChocolate.csproj"),
        Path.Combine("src", "Grpc", "Rocket.Surgery.LaunchPad.Grpc.csproj"),
        Path.Combine("src", "FluentValidation", "Rocket.Surgery.LaunchPad.FluentValidation.csproj"),
        Path.Combine("src", "FluentValidation.AspNetCore", "Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore.csproj"),
        Path.Combine("src", "FluentValidation.Blazor", "Rocket.Surgery.LaunchPad.FluentValidation.Blazor.csproj"),
        Path.Combine("src", "FluentValidation.Grpc", "Rocket.Surgery.LaunchPad.FluentValidation.Grpc.csproj"),
        Path.Combine("src", "ZeroAlloc.AspNetCore", "Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore.csproj"),
        Path.Combine("src", "ZeroAlloc.Blazor", "Rocket.Surgery.LaunchPad.ZeroAlloc.Blazor.csproj"),
        Path.Combine("src", "ZeroAlloc.Grpc", "Rocket.Surgery.LaunchPad.ZeroAlloc.Grpc.csproj"),
        Path.Combine("src", "ZeroAlloc.HotChocolate", "Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate.csproj"),
    ];

    /// <summary>
    ///     Packs the packages, or returns the pack of this run.
    /// </summary>
    /// <returns>The packages.</returns>
    public static Task<PackedSdk> Pack() => _packed.Value;

    /// <summary>
    ///     Deletes the feed. Called once when the test session ends.
    /// </summary>
    public static void Clean()
    {
        if (!_packed.IsValueCreated || !_packed.Value.IsCompletedSuccessfully) return;
        try
        {
            Directory.Delete(Directory.GetParent(_packed.Value.Result.Feed)!.FullName, recursive: true);
        }
        catch (IOException)
        {
            // best effort, the system cleans its temporary folder up
        }
    }

    /// <summary>
    ///     Replaces the versions that change from run to run (this run's packages) or with a dependency update (Dovetail's) in a text with
    ///     a placeholder, so snapshots hold what is stable.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>The text with placeholders.</returns>
    public string Scrub(string text) => text.Replace(Version, "{LaunchPadVersion}", StringComparison.Ordinal).Replace(DovetailVersion, "{DovetailVersion}", StringComparison.Ordinal);

    /// <summary>
    ///     The version of Dovetail the repository pins, which the SDK chains.
    /// </summary>
    public static string DovetailVersion => Repository.PackageVersion("Joinery.Dovetail.Sdk", "net11.0");

    private static async Task<PackedSdk> PackCore()
    {
        var root = Path.Combine(Path.GetTempPath(), "launchpad-sdk-tests", $"pack-{Guid.NewGuid():N}");
        var feed = Path.Combine(root, "feed");
        Directory.CreateDirectory(feed);

        // All the projects are packed by one build, so each project they share (Foundation, the analyzers) is built once and the projects
        // build in parallel; packing them one dotnet pack at a time built every project's references again for each of them. ArtifactsPath
        // keeps the intermediate files of this pack out of the projects' obj folders, where a build running at the same time (a developer's,
        // or the other target framework of this test project) would collide with it. The package icon is a file the release pipeline puts
        // in .temp, which a checkout does not have, and packing fails without it: nothing here looks at the icon.
        var version = $"0.0.0-sdktests.{Guid.NewGuid().ToString("N")[..8]}";
        var traversal = Path.Combine(root, "pack.proj");
        await File.WriteAllTextAsync(traversal, PackProject(Path.Combine(root, "artifacts"), feed, version));
        await DotnetCli.Run($"msbuild \"{traversal}\" -nologo -m -nodeReuse:false", Repository.Root);

        using var archive = ZipFile.OpenRead(Path.Combine(feed, $"{SdkPackageId}.{version}.nupkg"));
        var files = archive
                   .Entries
                   .Select(z => z.FullName)
                   .Where(z => !z.StartsWith("_rels/", StringComparison.Ordinal) && !z.StartsWith("package/", StringComparison.Ordinal) && !z.StartsWith("images/", StringComparison.Ordinal) && z != "[Content_Types].xml")
                   .Order(StringComparer.Ordinal)
                   .ToArray();
        using var reader = new StreamReader(archive.GetEntry("Sdk/Packages.props")?.Open() ?? throw new InvalidOperationException("The SDK has no Sdk/Packages.props"));
        return new(feed, version, files, await reader.ReadToEndAsync());
    }

    /// <summary>
    ///     A traversal project that packs <see cref="Projects" /> in one parallel build, the way <c>sample/Sample.Sdk/build/pack.proj</c>
    ///     does: restore first, with a global property of its own so the pack evaluates the projects again and sees what the restore wrote.
    /// </summary>
    /// <param name="artifacts">The artifacts folder of the build.</param>
    /// <param name="feed">The folder the packages are written to.</param>
    /// <param name="version">The version of the packages.</param>
    /// <returns>The text of the project.</returns>
    private static string PackProject(string artifacts, string feed, string version)
    {
        var properties = $"Configuration=Release;ArtifactsPath={artifacts};PackageOutputPath={feed};Version={version};PackageVersion={version};PackageIcon=";
        return new XDocument(
            new XElement(
                "Project",
                new XAttribute("DefaultTargets", "Pack"),
                new XElement("ItemGroup", Projects.Select(z => new XElement("PackProject", new XAttribute("Include", Path.Combine(Repository.Root, z))))),
                new XElement(
                    "Target",
                    new XAttribute("Name", "Pack"),
                    new XElement(
                        "MSBuild",
                        new XAttribute("Projects", "@(PackProject)"),
                        new XAttribute("Targets", "Restore"),
                        new XAttribute("Properties", $"{properties};_SdkTestsRestore=true")
                    ),
                    new XElement(
                        "MSBuild",
                        new XAttribute("Projects", "@(PackProject)"),
                        new XAttribute("Targets", "Pack"),
                        new XAttribute("Properties", properties),
                        new XAttribute("BuildInParallel", "true")
                    )
                )
            )
        ).ToString();
    }

    private static readonly Lazy<Task<PackedSdk>> _packed = new(PackCore);
}
