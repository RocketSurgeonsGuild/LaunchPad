using System.Text.Json.Nodes;

using Hosting.Tests.Scaffolding;

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
    ///     The target frameworks the repository declares (<c>LaunchPadTargetFrameworks</c> in <c>Directory.Build.props</c>), which the
    ///     throwaway projects target in turn (<see cref="TargetFrameworksAttribute" />).
    /// </summary>
    public static IReadOnlyList<string> TargetFrameworks { get; } = PinnedPackages.TargetFrameworks(Root);

    /// <summary>
    ///     The version <c>Directory.Packages.props</c> pins for <paramref name="packageId" /> on <paramref name="targetFramework" />.
    /// </summary>
    /// <param name="packageId">The package id.</param>
    /// <param name="targetFramework">The target framework, one of <see cref="TargetFrameworks" />.</param>
    /// <returns>The pinned version.</returns>
    public static string PackageVersion(string packageId, string targetFramework) => PinnedPackages.Version(Root, packageId, targetFramework);

    private static string FindRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "LaunchPad.slnx"))) return directory.FullName;
        }

        throw new InvalidOperationException($"LaunchPad.slnx was not found above {AppContext.BaseDirectory}");
    }
}
