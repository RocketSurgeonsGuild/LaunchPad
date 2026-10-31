using System.IO.Compression;
using System.Xml.Linq;

namespace Hosting.Tests.Scaffolding;

/// <summary>
///     <c>Rocket.Surgery.LaunchPad.Hosting</c>, packed from the repository into a feed of its own, once for all the tests. The version is
///     unique to the run, so a copy of the package an earlier run left in a packages folder cannot stand in for this one.
/// </summary>
/// <param name="Feed">The folder holding the package; a folder with .nupkg files is a NuGet feed.</param>
/// <param name="PackageId">The package id.</param>
/// <param name="Version">The package version.</param>
/// <param name="Files">The files in the package, without the package bookkeeping.</param>
/// <param name="Dependencies">The ids of the packages it depends on, by target framework.</param>
internal sealed record PackedHosting(
    string Feed,
    string PackageId,
    string Version,
    IReadOnlyList<string> Files,
    IReadOnlyDictionary<string, IReadOnlyList<string>> Dependencies
)
{
    /// <summary>
    ///     Packs the package, or returns the pack of this run.
    /// </summary>
    /// <returns>The package.</returns>
    public static Task<PackedHosting> Pack() => _packed.Value;

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

    private static async Task<PackedHosting> PackCore()
    {
        var root = Path.Combine(Path.GetTempPath(), "launchpad-hosting-tests", $"pack-{Guid.NewGuid():N}");
        var feed = Path.Combine(root, "feed");
        Directory.CreateDirectory(feed);

        // --artifacts-path keeps the intermediate files of this pack out of src/Hosting/obj, where a build running at the same time (a
        // developer's, or the other target framework of this test project) would collide with it. The package icon is a file the release
        // pipeline puts in .temp, which a checkout does not have, and packing fails without it: nothing here looks at the icon.
        var version = $"0.0.0-hostingtests.{Guid.NewGuid().ToString("N")[..8]}";
        await DotnetCli.Run(
            $"pack \"{Path.Combine(Repository.Root, "src", "Hosting", "Rocket.Surgery.LaunchPad.Hosting.csproj")}\" --nologo --artifacts-path \"{Path.Combine(root, "artifacts")}\" -o \"{feed}\" -p:Version={version} -p:PackageVersion={version} -p:PackageIcon=",
            Repository.Root
        );

        var nupkg = Directory.EnumerateFiles(feed, "*.nupkg").Single();
        using var archive = ZipFile.OpenRead(nupkg);
        var nuspec = archive.Entries.Single(z => z.FullName.EndsWith(".nuspec", StringComparison.Ordinal));
        using var reader = nuspec.Open();
        var document = XDocument.Load(reader);
        var ns = document.Root!.Name.Namespace;
        var metadata = document.Root.Element(ns + "metadata")!;

        var dependencies = metadata
                          .Element(ns + "dependencies")
                         ?.Elements(ns + "group")
                          .ToDictionary(
                               z => (string?)z.Attribute("targetFramework") ?? "any",
                               z => (IReadOnlyList<string>)[.. z.Elements(ns + "dependency").Select(d => (string)d.Attribute("id")!).Order(StringComparer.Ordinal)]
                           )
                   ?? [];
        var files = archive
                   .Entries
                   .Select(z => z.FullName)
                   .Where(z => !z.StartsWith("_rels/", StringComparison.Ordinal) && !z.StartsWith("package/", StringComparison.Ordinal) && z != "[Content_Types].xml")
                   .Order(StringComparer.Ordinal)
                   .ToArray();
        return new(feed, (string)metadata.Element(ns + "id")!, (string)metadata.Element(ns + "version")!, files, dependencies);
    }

    private static readonly Lazy<Task<PackedHosting>> _packed = new(PackCore);
}
