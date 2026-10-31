using System.Text.Json;

using Hosting.Tests.Scaffolding;

using Microsoft.Build.Utilities.ProjectCreation;

namespace Sdk.Tests.Scaffolding;

/// <summary>
///     What a project ends up with once the SDK has run: the hosts Dovetail found and the packages the project references.
/// </summary>
/// <param name="Hosts">The <c>DovetailHost</c> items, which are the host models Dovetail detected.</param>
/// <param name="PackageReferences">
///     Every package the project declares itself, as <c>Id</c>, and every package the LaunchPad or Dovetail SDK added to it, as
///     <c>Id Version (SDK)</c>. The packages the .NET SDK adds on its own are left out.
/// </param>
internal sealed record SdkEvaluation(IReadOnlyList<string> Hosts, IReadOnlyList<string> PackageReferences);

/// <summary>
///     A folder of throwaway projects that apply the packed <c>Rocket.Surgery.LaunchPad.Sdk</c> in their <c>Directory.Build.props</c> and pin
///     it in <c>global.json</c>, as an application outside this repository does. MSBuild.ProjectCreation writes the files: the projects, the
///     <c>global.json</c>, and the <c>NuGet.config</c> of a package repository of the tree's own, which restores the LaunchPad packages from
///     the packed feed and keeps what it restores out of the packages folder of the machine. The projects are built with the <c>dotnet</c>
///     CLI, out of process, as a developer builds them.
/// </summary>
/// <remarks>Modelled on <c>SdkTestProject</c> in Dovetail's SDK tests.</remarks>
internal sealed class SdkTree : IDisposable
{
    /// <summary>
    ///     Creates the tree.
    /// </summary>
    /// <param name="packages">The packed SDK and the packages it adds.</param>
    /// <param name="targetFramework">The target framework of the projects of the tree.</param>
    public SdkTree(PackedSdk packages, string targetFramework)
    {
        _packages = packages;
        TargetFramework = targetFramework;
        Directory = System.IO.Directory.CreateDirectory(Path.Combine(ResolveLinks(Path.GetTempPath()), "launchpad-sdk-tests", $"app-{Guid.NewGuid():N}")).FullName;

        // NuGet.config with the packed feed and nuget.org, and a packages folder in the tree
        _repository = PackageRepository.Create(Directory, new Uri(packages.Feed), new Uri("https://api.nuget.org/v3/index.json"));

        // the .NET SDK of the repository, and the LaunchPad SDK of this run
        GlobalJsonCreator
           .Create(
                Directory,
                (string)Repository.DotnetSdk["version"]!,
                GlobalJsonSdkRollForward.LatestFeature,
                (bool?)Repository.DotnetSdk["allowPrerelease"]
            )
           .MSBuildSdk(PackedSdk.SdkPackageId, packages.Version)
           .Save();

        // The one line of LaunchPad wiring an application has: the SDK, applied to every project below the file. The packages folder of the
        // machine is a fallback the restores only read, so the packages nuget.org has are not downloaded again for every tree.
        AddFile(
            "Directory.Build.props",
            ProjectCreator
               .Create(sdk: PackedSdk.SdkPackageId)
               .Property("NuGetAudit", "false")
               .Property("RestoreAdditionalProjectFallbackFolders", $"$(RestoreAdditionalProjectFallbackFolders);{GlobalPackagesFolder}")
        );
        // the project files in the tree stand alone: nothing above them may add to them
        AddFile("Directory.Build.targets", ProjectCreator.Create());
        AddFile("Directory.Packages.props", ProjectCreator.Create());
    }

    /// <summary>
    ///     The folder.
    /// </summary>
    public string Directory { get; }

    /// <summary>
    ///     The target framework of the projects of the tree.
    /// </summary>
    public string TargetFramework { get; }

    /// <summary>
    ///     Writes an MSBuild file.
    /// </summary>
    /// <param name="relativePath">The path in the tree.</param>
    /// <param name="project">The file, created without a path: MSBuild.ProjectCreation shares a project of one path between the tests.</param>
    /// <returns>This tree.</returns>
    public SdkTree AddFile(string relativePath, ProjectCreator project)
    {
        project.Save(Path.Combine(Directory, relativePath));
        return this;
    }

    /// <summary>
    ///     Writes a project of a host model into a folder of the tree named after it, with the <c>Program.cs</c> of the host model.
    /// </summary>
    /// <param name="name">The name of the project and of its folder.</param>
    /// <param name="host">The host model.</param>
    /// <param name="project">The project, when the test adds to <see cref="HostModelShape.Project" />.</param>
    /// <returns>This tree.</returns>
    public SdkTree AddProject(string name, HostModel host, ProjectCreator? project = null)
    {
        AddFile($"{name}/{name}.csproj", project ?? HostModelShape.Project(host, TargetFramework));
        File.WriteAllText(Path.Combine(Directory, name, "Program.cs"), HostModelShape.Of(host).Program);
        return this;
    }

    /// <summary>
    ///     Evaluates a project, which is all the SDK needs to choose the packages: NuGet restores what the evaluation declares.
    /// </summary>
    /// <param name="name">The name of the project.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>What the project ends up with.</returns>
    public async Task<SdkEvaluation> Evaluate(string name, CancellationToken cancellationToken = default)
    {
        var output = await DotnetCli.Run($"msbuild \"{ProjectPath(name)}\" -nologo -getItem:PackageReference -getItem:DovetailHost", Directory, null, cancellationToken);
        // -getItem writes JSON after anything the SDK resolver logs while it restores the SDKs
        using var json = JsonDocument.Parse(output[output.IndexOf('{', StringComparison.Ordinal)..]);
        var items = json.RootElement.GetProperty("Items");

        var hosts = ItemsNamed(items, "DovetailHost").Select(z => z.GetProperty("Identity").GetString()!).Order(StringComparer.Ordinal).ToArray();
        var packageReferences = ItemsNamed(items, "PackageReference")
                               .Select(
                                    z => (
                                        Id: z.GetProperty("Identity").GetString()!,
                                        Version: z.TryGetProperty("Version", out var version) ? version.GetString() : null,
                                        Implicit: z.TryGetProperty("IsImplicitlyDefined", out var isImplicit) && isImplicit.GetString() == "true"
                                    )
                                )
                               .Where(z => !z.Implicit || z.Id.StartsWith("Rocket.Surgery.LaunchPad.", StringComparison.Ordinal) || z.Id.StartsWith("Joinery.", StringComparison.Ordinal))
                               .Select(z => z.Implicit ? _packages.Scrub($"{z.Id} {z.Version} (SDK)") : z.Id)
                               .Order(StringComparer.Ordinal)
                               .ToArray();
        return new(hosts, packageReferences);
    }

    /// <summary>
    ///     Builds a project.
    /// </summary>
    /// <param name="name">The name of the project.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The output of the build.</returns>
    public Task<string> Build(string name, CancellationToken cancellationToken = default) =>
        DotnetCli.Run($"build \"{ProjectPath(name)}\" --nologo -nodeReuse:false", Directory, null, cancellationToken);

    /// <summary>
    ///     Runs a project that was built, and returns what it wrote.
    /// </summary>
    /// <param name="name">The name of the project.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The output of the application.</returns>
    public Task<string> Run(string name, CancellationToken cancellationToken = default) =>
        DotnetCli.Run($"\"{Path.Combine(Directory, name, "bin", "Debug", TargetFramework, $"{name}.dll")}\"", Directory, cancellationToken: cancellationToken);

    /// <inheritdoc />
    public void Dispose()
    {
        _repository.Dispose();

        // set SDK_TESTS_KEEP to look at what a failing test built
        if (Environment.GetEnvironmentVariable("SDK_TESTS_KEEP") is { Length: > 0, }) return;

        try
        {
            System.IO.Directory.Delete(Directory, recursive: true);
        }
        catch (IOException)
        {
            // best effort, the system cleans its temporary folder up
        }
    }

    private static IEnumerable<JsonElement> ItemsNamed(JsonElement items, string name) => items.TryGetProperty(name, out var list) ? list.EnumerateArray() : [];

    /// <summary>
    ///     The path with its symbolic links resolved. macOS keeps its temporary folder behind <c>/var</c>, a link to <c>/private/var</c>:
    ///     MSBuild reports the project of a project reference under the resolved path, and NuGet leaves a project it finds under another
    ///     path than the reference's out of the restore.
    /// </summary>
    private static string ResolveLinks(string path)
    {
        if (OperatingSystem.IsWindows()) return path;

        var resolved = Path.DirectorySeparatorChar.ToString();
        foreach (var segment in path.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries))
        {
            resolved = Path.Combine(resolved, segment);
            if (new DirectoryInfo(resolved).ResolveLinkTarget(returnFinalTarget: true) is { } target) resolved = target.FullName;
        }

        return resolved;
    }

    private string ProjectPath(string name) => Path.Combine(Directory, name, $"{name}.csproj");

    /// <summary>
    ///     The packages folder of the machine, which the tree only reads.
    /// </summary>
    private static string GlobalPackagesFolder { get; } =
        Environment.GetEnvironmentVariable("NUGET_PACKAGES") is { Length: > 0, } folder
            ? folder
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages");

    private readonly PackedSdk _packages;
    private readonly PackageRepository _repository;
}
