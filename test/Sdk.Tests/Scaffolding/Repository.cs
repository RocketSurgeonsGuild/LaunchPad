using System.Text.Json.Nodes;
using System.Xml.Linq;

namespace Sdk.Tests.Scaffolding;

/// <summary>
///     The repository the tests run in: where it is, the .NET SDK it builds with, and the package versions it pins, so the throwaway projects
///     use exactly what the repository does.
/// </summary>
internal static class Repository
{
    /// <summary>
    ///     The root of the repository, found from the test assembly (the folder with <c>LaunchPad.slnx</c>).
    /// </summary>
    public static string Root { get; } = FindRoot();

    /// <summary>
    ///     The <c>sdk</c> section of the repository's <c>global.json</c>.
    /// </summary>
    public static JsonObject DotnetSdk { get; } = JsonNode.Parse(File.ReadAllText(Path.Combine(Root, "global.json")))!["sdk"]!.AsObject();

    /// <summary>
    ///     The version <c>Directory.Packages.props</c> pins for <paramref name="packageId" /> on <paramref name="targetFramework" />. The
    ///     Microsoft packages are pinned once per release line, in item groups conditioned on the target framework, and the packages every
    ///     project gets are pinned as <c>GlobalPackageReference</c>.
    /// </summary>
    /// <param name="packageId">The package id.</param>
    /// <param name="targetFramework">The target framework (<c>net10.0</c> or <c>net11.0</c>).</param>
    /// <returns>The pinned version.</returns>
    public static string PackageVersion(string packageId, string targetFramework)
    {
        var document = XDocument.Load(Path.Combine(Root, "Directory.Packages.props"));
        var net11 = targetFramework == "net11.0";
        foreach (var group in document.Root!.Elements("ItemGroup"))
        {
            // the net10.0 line is conditioned on !IsTargetFrameworkCompatible(..., 'net11.0'), the net11.0 line on the same without the !
            if (( (string?)group.Attribute("Condition") )?.Trim() is { Length: > 0, } condition && condition.StartsWith('!') == net11) continue;

            foreach (var item in group.Elements().Where(z => z.Name.LocalName is "PackageVersion" or "GlobalPackageReference" && (string?)z.Attribute("Include") == packageId))
            {
                return (string?)item.Attribute("Version") ?? item.Element("Version")?.Value ?? throw new InvalidOperationException($"{packageId} has no version");
            }
        }

        throw new InvalidOperationException($"Directory.Packages.props does not pin {packageId} for {targetFramework}");
    }

    private static string FindRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "LaunchPad.slnx"))) return directory.FullName;
        }

        throw new InvalidOperationException($"LaunchPad.slnx was not found above {AppContext.BaseDirectory}");
    }
}
