using System.Text.Json;
using System.Text.Json.Nodes;

using Hosting.Tests.Scaffolding;

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
///     it in <c>global.json</c>, as an application outside this repository does. The tree sees nothing of the repository, restores the
///     LaunchPad packages from the packed feed, and keeps the packages it restores out of the packages folder of the machine.
/// </summary>
internal sealed class SdkTree : IDisposable
{
    public SdkTree(PackedSdk packages)
    {
        _packages = packages;
        Directory = System.IO.Directory.CreateDirectory(Path.Combine(ResolveLinks(Path.GetTempPath()), "launchpad-sdk-tests", $"app-{Guid.NewGuid():N}")).FullName;

        // The one line of LaunchPad wiring an application has: the SDK, applied to every project below the file.
        AddFile("Directory.Build.props", $$"""
            <Project Sdk="{{PackedSdk.SdkPackageId}}">
              <PropertyGroup>
                <NuGetAudit>false</NuGetAudit>
                <RestoreAdditionalProjectFallbackFolders>$(RestoreAdditionalProjectFallbackFolders);{{GlobalPackagesFolder}}</RestoreAdditionalProjectFallbackFolders>
              </PropertyGroup>
            </Project>
            """);
        AddFile("Directory.Build.targets", "<Project />");
        AddFile("Directory.Packages.props", "<Project />");

        // the .NET SDK of the repository, and the LaunchPad SDK of this run
        var globalJson = JsonNode.Parse(File.ReadAllText(Path.Combine(Repository.Root, "global.json")))!.AsObject();
        globalJson.Remove("test");
        globalJson["msbuild-sdks"] = new JsonObject { [PackedSdk.SdkPackageId] = packages.Version, };
        AddFile("global.json", globalJson.ToJsonString(new() { WriteIndented = true, }));

        AddFile("nuget.config", $"""
            <?xml version="1.0" encoding="utf-8" ?>
            <configuration>
              <packageSources>
                <clear />
                <add key="sdk-tests" value="{packages.Feed}" />
                <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
              </packageSources>
              <packageSourceMapping>
                <packageSource key="sdk-tests">
                  <package pattern="Rocket.Surgery.LaunchPad.*" />
                </packageSource>
                <packageSource key="nuget.org">
                  <package pattern="*" />
                </packageSource>
              </packageSourceMapping>
            </configuration>
            """);
    }

    /// <summary>
    ///     The folder.
    /// </summary>
    public string Directory { get; }

    /// <summary>
    ///     Turns central package management on, as most applications have it: the projects added after this leave their versions to the
    ///     tree's <c>Directory.Packages.props</c>, and the packages the SDK adds keep the versions it gives them.
    /// </summary>
    /// <returns>This tree.</returns>
    public SdkTree WithCentralPackageManagement()
    {
        _centralVersions = [with(StringComparer.OrdinalIgnoreCase)];
        return AddFile("Directory.Packages.props", "<Project><PropertyGroup><ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally></PropertyGroup></Project>");
    }

    /// <summary>
    ///     Writes a file.
    /// </summary>
    /// <param name="relativePath">The path in the tree.</param>
    /// <param name="content">The content.</param>
    /// <returns>This tree.</returns>
    public SdkTree AddFile(string relativePath, string content)
    {
        var path = Path.Combine(Directory, relativePath);
        System.IO.Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content);
        return this;
    }

    /// <summary>
    ///     Writes an application (or a library) into a folder of the tree, named after the folder.
    /// </summary>
    /// <param name="name">The name of the project and of its folder.</param>
    /// <param name="project">The project.</param>
    /// <returns>This tree.</returns>
    public SdkTree AddProject(string name, SdkApp project)
    {
        if (_centralVersions is { } versions)
        {
            foreach (var (id, version) in project.Packages) versions[id] = version;
            AddFile(
                "Directory.Packages.props",
                $"""
                <Project>
                  <PropertyGroup>
                    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
                  </PropertyGroup>
                  <ItemGroup>
                {string.Concat(versions.Select(z => $"""    <PackageVersion Include="{z.Key}" Version="{z.Value}" />{Environment.NewLine}"""))}  </ItemGroup>
                </Project>
                """
            );
        }

        AddFile($"{name}/{name}.csproj", project.ToXml(Repository.TargetFramework, _centralVersions is not null));
        AddFile($"{name}/Program.cs", project.Program);
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
        var output = await DotnetCli.Run(
            $"msbuild \"{ProjectPath(name)}\" -nologo -getItem:PackageReference -getItem:DovetailHost",
            Directory,
            BuildEnvironment,
            cancellationToken
        );
        // -getItem writes JSON after anything the SDK resolver logs while it restores the SDKs
        using var json = JsonDocument.Parse(output[output.IndexOf('{', StringComparison.Ordinal)..]);
        var items = json.RootElement.GetProperty("Items");

        var hosts = Items(items, "DovetailHost").Select(z => z.GetProperty("Identity").GetString()!).Order(StringComparer.Ordinal).ToArray();
        var packageReferences = Items(items, "PackageReference")
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
        DotnetCli.Run($"build \"{ProjectPath(name)}\" --nologo -nodeReuse:false", Directory, BuildEnvironment, cancellationToken);

    /// <summary>
    ///     Runs a project that was built, and returns what it wrote.
    /// </summary>
    /// <param name="name">The name of the project.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The output of the application.</returns>
    public Task<string> Run(string name, CancellationToken cancellationToken = default) =>
        DotnetCli.Run(
            $"\"{Path.Combine(Directory, name, "bin", "Debug", Repository.TargetFramework, $"{name}.dll")}\"",
            Directory,
            cancellationToken: cancellationToken
        );

    /// <inheritdoc />
    public void Dispose()
    {
        // set SDK_TESTS_KEEP to look at what a failing test built
        if (Environment.GetEnvironmentVariable("SDK_TESTS_KEEP") is { Length: > 0 }) return;

        try
        {
            System.IO.Directory.Delete(Directory, recursive: true);
        }
        catch (IOException)
        {
            // best effort, the system cleans its temporary folder up
        }
    }

    private static IEnumerable<JsonElement> Items(JsonElement items, string name) =>
        items.TryGetProperty(name, out var list) ? list.EnumerateArray() : [];

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
        Environment.GetEnvironmentVariable("NUGET_PACKAGES") is { Length: > 0 } folder
            ? folder
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages");

    /// <summary>
    ///     The packages folder of this tree, which the SDK resolver restores the SDKs into as well.
    /// </summary>
    private Dictionary<string, string?> BuildEnvironment => new() { ["NUGET_PACKAGES"] = Path.Combine(Directory, ".packages"), };

    private readonly PackedSdk _packages;
    private Dictionary<string, string>? _centralVersions;
}
