using System.Xml.Linq;

namespace Hosting.Tests.Scaffolding;

/// <summary>
///     The repository the tests run in: where it is, and the package versions it pins, so the throwaway applications reference exactly
///     what the samples do.
/// </summary>
internal static class Repository
{
    /// <summary>
    ///     The root of the repository, found from the test assembly (the folder with <c>LaunchPad.slnx</c>).
    /// </summary>
    public static string Root { get; } = FindRoot();

    /// <summary>
    ///     The target framework the tests run on (<c>net10.0</c> or <c>net11.0</c>), which the throwaway applications target as well, so
    ///     running the tests on both frameworks covers both.
    /// </summary>
    public static string TargetFramework { get; } = FindTargetFramework();

    /// <summary>
    ///     The version <c>Directory.Packages.props</c> pins for <paramref name="packageId" /> on <see cref="TargetFramework" />. The
    ///     Microsoft packages are pinned once per release line, in item groups conditioned on the target framework, and the packages every
    ///     project gets are pinned as <c>GlobalPackageReference</c>.
    /// </summary>
    /// <param name="packageId">The package id.</param>
    /// <returns>The pinned version.</returns>
    public static string PackageVersion(string packageId)
    {
        var document = XDocument.Load(Path.Combine(Root, "Directory.Packages.props"));
        var net11 = TargetFramework == "net11.0";
        foreach (var group in document.Root!.Elements("ItemGroup"))
        {
            // the net10.0 line is conditioned on !IsTargetFrameworkCompatible(..., 'net11.0'), the net11.0 line on the same without the !
            if (( (string?)group.Attribute("Condition") )?.Trim() is { Length: > 0 } condition && condition.StartsWith('!') == net11) continue;

            foreach (var item in group.Elements().Where(z => z.Name.LocalName is "PackageVersion" or "GlobalPackageReference" && (string?)z.Attribute("Include") == packageId))
            {
                return (string?)item.Attribute("Version") ?? item.Element("Version")?.Value ?? throw new InvalidOperationException($"{packageId} has no version");
            }
        }

        throw new InvalidOperationException($"Directory.Packages.props does not pin {packageId} for {TargetFramework}");
    }

    private static string FindRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "LaunchPad.slnx"))) return directory.FullName;
        }

        throw new InvalidOperationException($"LaunchPad.slnx was not found above {AppContext.BaseDirectory}");
    }

    private static string FindTargetFramework()
    {
        // ".NETCoreApp,Version=v10.0" is the framework the test assembly was compiled for, which stays put when the runtime rolls forward
        var name = AppContext.TargetFrameworkName ?? throw new InvalidOperationException("The test assembly has no target framework");
        return $"net{name[( name.IndexOf("Version=v", StringComparison.Ordinal) + "Version=v".Length )..]}";
    }
}
