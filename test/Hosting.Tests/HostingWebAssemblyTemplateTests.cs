using Hosting.Tests.Scaffolding;

namespace Hosting.Tests;

/// <summary>
///     Blazor WebAssembly, the browser host model, gets what <c>Rocket.Surgery.LaunchPad.WebAssembly.Hosting</c> used to ship (folded into
///     <c>Rocket.Surgery.LaunchPad.Hosting</c>): browser console logging, the <c>Environment</c> and <c>BaseAddress</c> enrichment, and the
///     ban on the generic hosting namespace, which the browser has no host of. A WebAssembly application cannot run on the machine the tests
///     do, so the tests read what was written into it and whether it builds.
/// </summary>
public class HostingWebAssemblyTemplateTests
{
    private const string BrowserConsoleLoggingJoint = "LaunchPadBrowserConsoleLoggingJoint";
    private const string WebAssemblyEnvironmentLoggingJoint = "LaunchPadWebAssemblyEnvironmentLoggingJoint";

    /// <summary>
    ///     Uses the generic hosting namespace, which a browser application must not.
    /// </summary>
    private const string GenericHostingProgram = """
        System.Console.WriteLine(typeof(Microsoft.Extensions.Hosting.IHostEnvironment).Name);
        """;

    private static CancellationToken Cancellation => TestContext.Current!.Execution.CancellationToken;

    /// <summary>
    ///     The application references Serilog through Dovetail, the browser console sink, and what a server application references as well
    ///     (the health checks, the console sink): it gets the browser templates, and nothing for a server.
    /// </summary>
    [Test]
    public async Task Should_Give_A_Blazor_WebAssembly_Application_The_Browser_Templates_And_Nothing_Server_Only()
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        tree.AddProject("app", BrowserApplication(package).AddPackage(AppProject.HealthChecks).AddPackage("Serilog.Sinks.Console"));

        await tree.Build("app", Cancellation);

        await Verify(new { Host = HostModel.WebAssembly, Templates = tree.EmittedTemplates("app") });
    }

    /// <summary>
    ///     The text that lands in the application, which is compiled there against whatever it references.
    /// </summary>
    [Test]
    [Arguments(BrowserConsoleLoggingJoint)]
    [Arguments(WebAssemblyEnvironmentLoggingJoint)]
    public async Task Should_Write_The_Browser_Templates_As_Internal_Embedded_Core_Serilog_Joints(string template)
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        tree.AddProject("app", BrowserApplication(package));

        await tree.Build("app", Cancellation);

        await Verify(tree.EmittedTemplate("app", template), "cs");
    }

    /// <summary>
    ///     The reference gates: without Serilog there is no logger to configure, and without the browser console sink there is nothing to
    ///     write to it with (the enrichment still applies to whatever sinks the application has).
    /// </summary>
    [Test]
    [Arguments(false, false)]
    [Arguments(false, true)]
    [Arguments(true, false)]
    public async Task Should_Give_A_Blazor_WebAssembly_Application_Only_The_Browser_Templates_It_References(bool serilog, bool browserConsole)
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        tree.AddProject("app", BrowserApplication(package, serilog, browserConsole));

        await tree.Build("app", Cancellation);

        await Verify(new { Serilog = serilog, BrowserConsole = browserConsole, Templates = tree.EmittedTemplates("app") });
    }

    /// <summary>
    ///     The host gate: a server application that references the browser console sink is not a browser.
    /// </summary>
    [Test]
    [Arguments(HostModel.GenericHost)]
    [Arguments(HostModel.AspNetCore)]
    public async Task Should_Not_Give_A_Server_Application_The_Browser_Templates(HostModel host)
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        tree.AddProject(
            "app",
            AppProject
               .Standalone(host)
               .AddPackage(package.PackageId, package.Version)
               .AddPackage(AppProject.DovetailSerilog)
               .AddPackage(AppProject.BrowserConsole)
        );

        await tree.Build("app", Cancellation);

        tree.EmittedTemplates("app").ShouldNotContain(BrowserConsoleLoggingJoint);
        tree.EmittedTemplates("app").ShouldNotContain(WebAssemblyEnvironmentLoggingJoint);
    }

    /// <summary>
    ///     A Blazor WebAssembly application that references the package fails to build when its own code uses the generic hosting namespace
    ///     (with the banned API analyzer, which the repository gives every project).
    /// </summary>
    [Test]
    public async Task Should_Ban_The_Generic_Hosting_Namespace_In_A_Blazor_WebAssembly_Application()
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        tree.AddProject("app", BrowserApplication(package).AddPackage(AppProject.BannedApiAnalyzers));
        tree.AddFile("app/Program.cs", GenericHostingProgram);

        var failure = await Should.ThrowAsync<InvalidOperationException>(() => tree.Build("app", Cancellation));

        failure.Message.ShouldContain("error RS0030");
        failure.Message.ShouldContain("'IHostEnvironment' is banned");
    }

    /// <summary>
    ///     The ban is on the application's code: one that keeps off the generic host builds, although Dovetail would write its
    ///     <c>IHostEnvironment</c> helper into any project that references the hosting abstractions (as the package does). The analyzer
    ///     reads the helper's documentation (<c>see cref</c>) when the project generates a documentation file, as the samples do.
    /// </summary>
    [Test]
    public async Task Should_Build_A_Blazor_WebAssembly_Application_That_Keeps_Off_The_Generic_Host()
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        tree.AddProject(
            "app",
            BrowserApplication(package).AddPackage(AppProject.BannedApiAnalyzers).AddProperty("GenerateDocumentationFile", "true")
        );

        var output = await tree.Build("app", Cancellation);

        output.ShouldNotContain("RS0030");
        tree.EmittedTemplates("app").ShouldBe([BrowserConsoleLoggingJoint, WebAssemblyEnvironmentLoggingJoint]);
    }

    /// <summary>
    ///     The same through the repository's targets, which is how <c>Sample.BlazorWasm</c> gets them, with a project reference to Hosting
    ///     and a documentation file like the sample's. It runs on its own: the projects build <c>src/Hosting</c> in the repository.
    /// </summary>
    [Test]
    [NotInParallel("src/Hosting")]
    public async Task Should_Ban_Only_The_Code_Of_A_Blazor_WebAssembly_Application_With_A_Project_Reference_To_Hosting()
    {
        using var tree = ThrowawayTree.InsideTheRepository();
        tree.AddProject("clean", RepositoryBrowserApplication());
        tree.AddProject("banned", RepositoryBrowserApplication());
        tree.AddFile("banned/Program.cs", GenericHostingProgram);

        var clean = await tree.Build("clean", Cancellation);
        var failure = await Should.ThrowAsync<InvalidOperationException>(() => tree.Build("banned", Cancellation));

        clean.ShouldNotContain("RS0030");
        tree.EmittedTemplates("clean").ShouldBe([BrowserConsoleLoggingJoint, WebAssemblyEnvironmentLoggingJoint]);
        failure.Message.ShouldContain("Program.cs(1,26): error RS0030");

        static AppProject RepositoryBrowserApplication() =>
            AppProject
               .Central(HostModel.WebAssembly)
               .AddPackage(AppProject.DovetailSerilog)
               .AddPackage(AppProject.BrowserConsole)
               .AddProjectReference(ThrowawayTree.HostingProject)
               .AddProperty("GenerateDocumentationFile", "true");
    }

    /// <summary>
    ///     The ban is only for the browser applications that reference the package themselves: a server application has a generic host, and
    ///     a browser application that receives the package through a library is not one the package configures.
    /// </summary>
    [Test]
    [Arguments(HostModel.GenericHost, true)]
    [Arguments(HostModel.WebAssembly, false)]
    public async Task Should_Not_Ban_The_Generic_Hosting_Namespace_Elsewhere(HostModel host, bool referencePackage)
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        tree.AddProject("library", AppProject.Standalone(HostModel.None).AddPackage(package.PackageId, package.Version));
        var application = AppProject.Standalone(host).AddPackage(AppProject.BannedApiAnalyzers).AddProjectReference("../library/library.csproj");
        if (referencePackage) application.AddPackage(package.PackageId, package.Version);
        tree.AddProject("app", application);
        tree.AddFile("app/Program.cs", GenericHostingProgram);

        var output = await tree.Build("app", Cancellation);

        output.ShouldNotContain("RS0030");
    }

    /// <summary>
    ///     A Blazor WebAssembly application in a tree outside the repository, with the packed package and the Serilog references the browser
    ///     templates are for.
    /// </summary>
    private static AppProject BrowserApplication(PackedHosting package, bool serilog = true, bool browserConsole = true)
    {
        var application = AppProject.Standalone(HostModel.WebAssembly).AddPackage(package.PackageId, package.Version);
        if (serilog) application.AddPackage(AppProject.DovetailSerilog);
        if (browserConsole) application.AddPackage(AppProject.BrowserConsole);
        return application;
    }
}
