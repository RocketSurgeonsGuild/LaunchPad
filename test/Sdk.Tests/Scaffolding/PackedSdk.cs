using System.IO.Compression;

using Hosting.Tests.Scaffolding;

namespace Sdk.Tests.Scaffolding;

/// <summary>
///     <c>Rocket.Surgery.LaunchPad.Sdk</c> and the packages it adds to an application, packed from the repository into a feed of their own,
///     once for all the tests. The version is unique to the run, so a copy an earlier run left in a packages folder cannot stand in for it.
/// </summary>
/// <param name="Feed">The folder holding the packages; a folder with .nupkg files is a NuGet feed.</param>
/// <param name="Version">The version of every package in the feed.</param>
/// <param name="SdkFiles">The files in the SDK package, without the package bookkeeping.</param>
/// <param name="PackagesProps">The text of the props file the SDK generates when it is packed.</param>
internal sealed record PackedSdk(string Feed, string Version, IReadOnlyList<string> SdkFiles, string PackagesProps)
{
    /// <summary>
    ///     The id of the SDK package.
    /// </summary>
    public const string SdkPackageId = "Rocket.Surgery.LaunchPad.Sdk";

    /// <summary>
    ///     The projects packed into the feed: the SDK, and every package it can add to an application.
    /// </summary>
    private static readonly string[] Projects =
    [
        Path.Combine("src", "Sdk", "Rocket.Surgery.LaunchPad.Sdk.csproj"),
        Path.Combine("src", "Hosting", "Rocket.Surgery.LaunchPad.Hosting.csproj"),
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
    public static string DovetailVersion => Repository.PackageVersion("Joinery.Dovetail.Sdk");

    private static async Task<PackedSdk> PackCore()
    {
        var root = Path.Combine(Path.GetTempPath(), "launchpad-sdk-tests", $"pack-{Guid.NewGuid():N}");
        var feed = Path.Combine(root, "feed");
        Directory.CreateDirectory(feed);

        // --artifacts-path keeps the intermediate files of this pack out of the projects' obj folders, where a build running at the same
        // time (a developer's, or the other target framework of this test project) would collide with it. The package icon is a file the
        // release pipeline puts in .temp, which a checkout does not have, and packing fails without it: nothing here looks at the icon.
        var version = $"0.0.0-sdktests.{Guid.NewGuid().ToString("N")[..8]}";
        foreach (var project in Projects)
        {
            await DotnetCli.Run(
                $"pack \"{Path.Combine(Repository.Root, project)}\" --nologo --artifacts-path \"{Path.Combine(root, "artifacts", Path.GetFileNameWithoutExtension(project))}\" -o \"{feed}\" -p:Version={version} -p:PackageVersion={version} -p:PackageIcon=",
                Repository.Root
            );
        }

        using var archive = ZipFile.OpenRead(Path.Combine(feed, $"{SdkPackageId}.{version}.nupkg"));
        var files = archive
                   .Entries
                   .Select(z => z.FullName)
                   .Where(z => !z.StartsWith("_rels/", StringComparison.Ordinal) && !z.StartsWith("package/", StringComparison.Ordinal) && z != "[Content_Types].xml")
                   .Order(StringComparer.Ordinal)
                   .ToArray();
        using var reader = new StreamReader(archive.GetEntry("Sdk/Packages.props")?.Open() ?? throw new InvalidOperationException("The SDK has no Sdk/Packages.props"));
        return new(feed, version, files, await reader.ReadToEndAsync());
    }

    private static readonly Lazy<Task<PackedSdk>> _packed = new(PackCore);
}
