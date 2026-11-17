using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Hosting.Tests.Scaffolding;

/// <summary>
///     The package versions the repository pins in <c>Directory.Packages.props</c>, and the target frameworks it declares in
///     <c>Directory.Build.props</c>, read the way MSBuild would for one target framework. Shared by the Hosting and SDK tests.
/// </summary>
internal static partial class PinnedPackages
{
    /// <summary>
    ///     The target frameworks of <c>LaunchPadTargetFrameworks</c> in the repository's <c>Directory.Build.props</c>, oldest first.
    /// </summary>
    /// <param name="root">The root of the repository.</param>
    /// <returns>The target frameworks.</returns>
    public static IReadOnlyList<string> TargetFrameworks(string root) =>
        XDocument
           .Load(Path.Combine(root, "Directory.Build.props"))
           .Descendants("LaunchPadTargetFrameworks")
           .Single()
           .Value.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    /// <summary>
    ///     The version <c>Directory.Packages.props</c> pins for <paramref name="packageId" /> on <paramref name="targetFramework" />, as a
    ///     <c>PackageVersion</c> or a <c>GlobalPackageReference</c>. An item group without a condition applies to every framework; one
    ///     conditioned on <c>IsTargetFrameworkCompatible('$(TargetFramework)', 'netX.Y')</c>, negated or not, applies where that holds, so a
    ///     package pinned once per release line resolves to the line of <paramref name="targetFramework" />.
    /// </summary>
    /// <param name="root">The root of the repository.</param>
    /// <param name="packageId">The package id.</param>
    /// <param name="targetFramework">The target framework, <c>netX.Y</c>.</param>
    /// <returns>The pinned version.</returns>
    public static string Version(string root, string packageId, string targetFramework)
    {
        var document = XDocument.Load(Path.Combine(root, "Directory.Packages.props"));
        foreach (var group in document.Root!.Elements("ItemGroup"))
        {
            if (!Applies((string?)group.Attribute("Condition"), targetFramework)) continue;

            foreach (var item in group.Elements())
            {
                if (item.Name.LocalName is not ( "PackageVersion" or "GlobalPackageReference" or "SdkPinnedPackage" ) || (string?)item.Attribute("Include") != packageId) continue;

                return (string?)item.Attribute("Version") ?? item.Element("Version")?.Value ?? throw new InvalidOperationException($"{packageId} has no version");
            }
        }

        throw new InvalidOperationException($"Directory.Packages.props does not pin {packageId} for {targetFramework}");
    }

    private static bool Applies(string? condition, string targetFramework)
    {
        if (string.IsNullOrWhiteSpace(condition)) return true;

        var match = CompatibleCondition().Match(condition);
        if (!match.Success) throw new InvalidOperationException($"Directory.Packages.props has an item group condition the tests can't read: {condition}");

        var compatible = FrameworkVersion(targetFramework) >= FrameworkVersion(match.Groups["framework"].Value);
        return match.Groups["not"].Success != compatible;
    }

    private static Version FrameworkVersion(string targetFramework) =>
        System.Version.Parse(FrameworkPattern().Match(targetFramework) is { Success: true } match
            ? match.Groups["version"].Value
            : throw new InvalidOperationException($"{targetFramework} is not a netX.Y target framework"));

    [GeneratedRegex(@"^\s*(?<not>!)?\s*\$\(\[MSBuild\]::IsTargetFrameworkCompatible\('\$\(TargetFramework\)',\s*'(?<framework>[^']+)'\)\)\s*$")]
    private static partial Regex CompatibleCondition();

    [GeneratedRegex(@"^net(?<version>\d+\.\d+)")]
    private static partial Regex FrameworkPattern();
}
