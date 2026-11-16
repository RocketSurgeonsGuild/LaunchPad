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
    ///     The target framework the tests run on, one of <c>LaunchPadTargetFrameworks</c>, which the throwaway applications target as well,
    ///     so running the tests on every framework covers every framework.
    /// </summary>
    public static string TargetFramework { get; } = FindTargetFramework();

    /// <summary>
    ///     The version <c>Directory.Packages.props</c> pins for <paramref name="packageId" /> on <see cref="TargetFramework" />.
    /// </summary>
    /// <param name="packageId">The package id.</param>
    /// <returns>The pinned version.</returns>
    public static string PackageVersion(string packageId) => PinnedPackages.Version(Root, packageId, TargetFramework);

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
        // ".NETCoreApp,Version=v11.0" is the framework the test assembly was compiled for, which stays put when the runtime rolls forward
        var name = AppContext.TargetFrameworkName ?? throw new InvalidOperationException("The test assembly has no target framework");
        return $"net{name[( name.IndexOf("Version=v", StringComparison.Ordinal) + "Version=v".Length )..]}";
    }
}
