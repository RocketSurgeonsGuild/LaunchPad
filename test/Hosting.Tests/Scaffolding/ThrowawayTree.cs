namespace Hosting.Tests.Scaffolding;

/// <summary>
///     A folder of throwaway projects that the tests build with <c>dotnet build</c>, and read back which of the package's templates
///     landed in each project's <c>obj/</c>.
/// </summary>
internal sealed class ThrowawayTree : IDisposable
{
    /// <summary>
    ///     The prefix of the files <c>Rocket.Surgery.LaunchPad.Hosting</c> writes into a project (<c>OutputPrefix</c> of its templates).
    /// </summary>
    public const string TemplatePrefix = "LaunchPadHosting";

    private ThrowawayTree(string directory, bool inRepository, PackedHosting? package)
    {
        Directory = directory;
        _inRepository = inRepository;
        Package = package;
    }

    /// <summary>
    ///     The folder.
    /// </summary>
    public string Directory { get; }

    /// <summary>
    ///     The packed Hosting package the projects of this tree restore it from, for a tree outside the repository.
    /// </summary>
    public PackedHosting? Package { get; }

    /// <summary>
    ///     A tree that restores <c>Rocket.Surgery.LaunchPad.Hosting</c> from the packed package, as an application outside this repository
    ///     does. It sees nothing of the repository (its <c>Directory.Build.targets</c> included), and keeps the packages it restores out of
    ///     the packages folder of the machine.
    /// </summary>
    /// <param name="package">The packed Hosting package.</param>
    /// <returns>The tree.</returns>
    public static ThrowawayTree OutsideTheRepository(PackedHosting package)
    {
        var directory = System.IO.Directory.CreateDirectory(Path.Combine(ResolveLinks(Path.GetTempPath()), "launchpad-hosting-tests", $"app-{Guid.NewGuid():N}")).FullName;
        var tree = new ThrowawayTree(directory, false, package);

        // The project files in the tree stand alone: nothing above them may add to them.
        tree.AddFile("Directory.Build.props", $$"""
            <Project>
              <PropertyGroup>
                <NuGetAudit>false</NuGetAudit>
                <RestoreAdditionalProjectFallbackFolders>$(RestoreAdditionalProjectFallbackFolders);{{GlobalPackagesFolder}}</RestoreAdditionalProjectFallbackFolders>
              </PropertyGroup>
            </Project>
            """);
        tree.AddFile("Directory.Build.targets", "<Project />");
        tree.AddFile("Directory.Packages.props", "<Project />");
        tree.AddFile("global.json", File.ReadAllText(Path.Combine(Repository.Root, "global.json")));
        tree.AddFile("nuget.config", $"""
            <?xml version="1.0" encoding="utf-8" ?>
            <configuration>
              <packageSources>
                <clear />
                <add key="hosting-tests" value="{package.Feed}" />
                <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
              </packageSources>
              <packageSourceMapping>
                <packageSource key="hosting-tests">
                  <package pattern="{package.PackageId}" />
                </packageSource>
                <packageSource key="nuget.org">
                  <package pattern="*" />
                </packageSource>
              </packageSourceMapping>
            </configuration>
            """);
        return tree;
    }

    /// <summary>
    ///     A tree inside the repository (in its git-ignored <c>artifacts</c> folder), where the repository's <c>Directory.Build.props</c>,
    ///     <c>Directory.Build.targets</c> and <c>Directory.Packages.props</c> apply to the projects, as they do to the samples. Projects
    ///     reference the Hosting project, which imports no package assets.
    /// </summary>
    /// <returns>The tree.</returns>
    public static ThrowawayTree InsideTheRepository()
    {
        var directory = System.IO.Directory.CreateDirectory(Path.Combine(Repository.Root, "artifacts", "hosting-tests", Guid.NewGuid().ToString("N"))).FullName;
        return new(directory, true, null);
    }

    /// <summary>
    ///     The path of the Hosting project, for a project reference.
    /// </summary>
    public static string HostingProject { get; } = Path.Combine(Repository.Root, "src", "Hosting", "Rocket.Surgery.LaunchPad.Hosting.csproj");

    /// <summary>
    ///     Writes a file.
    /// </summary>
    /// <param name="relativePath">The path in the tree.</param>
    /// <param name="content">The content.</param>
    /// <returns>This tree.</returns>
    public ThrowawayTree AddFile(string relativePath, string content)
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
    public ThrowawayTree AddProject(string name, AppProject project)
    {
        AddFile($"{name}/{name}.csproj", project.ToXml(Repository.TargetFramework));
        AddFile($"{name}/Program.cs", project.Program);
        return this;
    }

    /// <summary>
    ///     Builds a project.
    /// </summary>
    /// <param name="name">The name of the project.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The output of the build.</returns>
    public Task<string> Build(string name, CancellationToken cancellationToken = default) =>
        DotnetCli.Run(
            $"build \"{Path.Combine(Directory, name, $"{name}.csproj")}\" --nologo -nodeReuse:false",
            Directory,
            _inRepository ? null : new Dictionary<string, string?> { ["NUGET_PACKAGES"] = PackagesFolder },
            cancellationToken
        );

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

    /// <summary>
    ///     The names of the templates of <c>Rocket.Surgery.LaunchPad.Hosting</c> that were written into a project, which is every
    ///     <c>LaunchPadHosting.*.g.cs</c> in its <c>obj/</c>.
    /// </summary>
    /// <param name="name">The name of the project.</param>
    /// <returns>The names, in order.</returns>
    public IReadOnlyList<string> EmittedTemplates(string name) =>
        EmittedFiles(name)
           .Select(Path.GetFileName)
           .OfType<string>()
           .Select(z => z[$"{TemplatePrefix}.".Length..^".g.cs".Length])
           .Order(StringComparer.Ordinal)
           .ToArray();

    /// <summary>
    ///     The text of a template that was written into a project.
    /// </summary>
    /// <param name="name">The name of the project.</param>
    /// <param name="template">The name of the template.</param>
    /// <returns>The generated file.</returns>
    public string EmittedTemplate(string name, string template) => File.ReadAllText(EmittedFiles(name).Single(z => Path.GetFileName(z) == $"{TemplatePrefix}.{template}.g.cs"));

    /// <summary>
    ///     Whether the build of a project wrote a file under its <c>bin</c>, which shows the project received the package.
    /// </summary>
    /// <param name="name">The name of the project.</param>
    /// <param name="fileName">The file name.</param>
    /// <returns><see langword="true" /> when the file was copied to the output.</returns>
    public bool HasOutput(string name, string fileName) => File.Exists(Path.Combine(Directory, name, "bin", "Debug", Repository.TargetFramework, fileName));

    /// <summary>
    ///     The <c>DovetailTemplate</c> items a project declares: which templates it knows about, whether or not it gets them.
    /// </summary>
    /// <param name="name">The name of the project.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The identities of the items.</returns>
    public async Task<IReadOnlyList<string>> DeclaredTemplates(string name, CancellationToken cancellationToken = default)
    {
        var output = await DotnetCli.Run(
            $"msbuild \"{Path.Combine(Directory, name, $"{name}.csproj")}\" -nologo -getItem:DovetailTemplate",
            Directory,
            _inRepository ? null : new Dictionary<string, string?> { ["NUGET_PACKAGES"] = PackagesFolder },
            cancellationToken
        );
        // -getItem writes JSON; the identities are the only thing needed of it
        var json = System.Text.Json.JsonDocument.Parse(output[output.IndexOf('{')..]);
        return json
              .RootElement
              .GetProperty("Items")
              .GetProperty("DovetailTemplate")
              .EnumerateArray()
              .Select(z => z.GetProperty("Identity").GetString()!)
              .Order(StringComparer.Ordinal)
              .ToArray();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        // set HOSTING_TESTS_KEEP to look at what a failing test built
        if (Environment.GetEnvironmentVariable("HOSTING_TESTS_KEEP") is { Length: > 0 }) return;

        try
        {
            System.IO.Directory.Delete(Directory, recursive: true);
        }
        catch (IOException)
        {
            // best effort, the system cleans its temporary folder up
        }
    }

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

    private IEnumerable<string> EmittedFiles(string name) =>
        System.IO.Directory.Exists(Path.Combine(Directory, name, "obj"))
            ? System.IO.Directory.EnumerateFiles(Path.Combine(Directory, name, "obj"), $"{TemplatePrefix}.*.g.cs", SearchOption.AllDirectories)
            : [];

    /// <summary>
    ///     The packages folder of the machine, which the trees outside the repository only read.
    /// </summary>
    private static string GlobalPackagesFolder { get; } =
        Environment.GetEnvironmentVariable("NUGET_PACKAGES") is { Length: > 0 } folder
            ? folder
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages");

    /// <summary>
    ///     The packages folder of this tree.
    /// </summary>
    private string PackagesFolder => Path.Combine(Directory, ".packages");

    private readonly bool _inRepository;
}
