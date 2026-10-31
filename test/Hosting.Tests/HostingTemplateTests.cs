using Hosting.Tests.Scaffolding;

namespace Hosting.Tests;

/// <summary>
///     The joints of <c>Rocket.Surgery.LaunchPad.Hosting</c> are templates: the package is packed, throwaway applications reference it
///     the way an application does and are built with <c>dotnet build</c>, and what the tests read is which of the package's templates
///     were written into each application's <c>obj/</c> (the build also proves the template compiles there) and what the application
///     does when it runs. Modelled on <c>HostingHelperTests</c> in Dovetail, whose package works the same way.
/// </summary>
public class HostingTemplateTests
{
    private const string HealthChecksJoint = "LaunchPadHealthChecksJoint";

    private static CancellationToken Cancellation => TestContext.Current!.Execution.CancellationToken;

    /// <summary>
    ///     The mechanism and its first user. The generic host needs the health check package to be referenced, and ASP.NET Core has them
    ///     in the framework it references, so both are applications the package can write the joint into.
    /// </summary>
    [Test]
    [Arguments(HostModel.GenericHost)]
    [Arguments(HostModel.AspNetCore)]
    public async Task Should_Give_An_Application_That_References_The_Package_The_HealthChecks_Joint(HostModel host)
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        tree.AddProject("app", Application(package, host));

        await tree.Build("app", Cancellation);
        var output = await tree.Run("app", Cancellation);

        await Verify(new { Host = host, Templates = tree.EmittedTemplates("app"), Output = output.Trim() });
    }

    /// <summary>
    ///     The text that lands in the application, which is compiled there against whatever it references.
    /// </summary>
    [Test]
    public async Task Should_Write_The_HealthChecks_Joint_As_An_Internal_Embedded_Core_Joint()
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        tree.AddProject("app", Application(package, HostModel.GenericHost));

        await tree.Build("app", Cancellation);

        await Verify(tree.EmittedTemplate("app", HealthChecksJoint), "cs");
    }

    /// <summary>
    ///     Blazor WebAssembly has no health checks. It is given none even when the application references the health check package
    ///     (another package may bring it along), which is what the host gate is for.
    /// </summary>
    [Test]
    public async Task Should_Not_Give_A_Host_Without_Health_Check_Support_The_HealthChecks_Joint()
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        tree.AddProject("app", Application(package, HostModel.WebAssembly));

        await tree.Build("app", Cancellation);

        await Verify(new { Host = HostModel.WebAssembly, Templates = tree.EmittedTemplates("app") });
    }

    /// <summary>
    ///     The reference gate: the generic host's application that does not reference the health check package gets no joint, and so no
    ///     health check services.
    /// </summary>
    [Test]
    public async Task Should_Not_Give_An_Application_That_Does_Not_Reference_Health_Checks_The_HealthChecks_Joint()
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        tree.AddProject("app", Application(package, HostModel.GenericHost, healthChecks: false));

        await tree.Build("app", Cancellation);
        var output = await tree.Run("app", Cancellation);

        await Verify(new { Host = HostModel.GenericHost, Templates = tree.EmittedTemplates("app"), Output = output.Trim() });
    }

    /// <summary>
    ///     A library has no host model to configure, and would export the joint to every application that references it.
    /// </summary>
    [Test]
    public async Task Should_Not_Give_A_Library_The_HealthChecks_Joint()
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        tree.AddProject("library", Application(package, HostModel.None).AddPackage(AppProject.HealthChecks));

        await tree.Build("library", Cancellation);

        await Verify(new { Project = "library", Templates = tree.EmittedTemplates("library") });
    }

    /// <summary>
    ///     The package is only for the projects that reference it themselves. <c>app</c> is an application that meets every other gate
    ///     (its host, the health check reference) and receives the package through <c>library</c>: the package's assets do reach it, and
    ///     it declares the template, but it is written nowhere, so the application has no health check services. <c>library</c>'s own
    ///     application, which references the package itself, is the control.
    /// </summary>
    [Test]
    [Arguments(HostModel.GenericHost)]
    [Arguments(HostModel.AspNetCore)]
    public async Task Should_Not_Give_An_Application_That_Receives_The_Package_Transitively_The_HealthChecks_Joint(HostModel host)
    {
        var package = await PackedHosting.Pack();
        using var tree = ThrowawayTree.OutsideTheRepository(package);
        tree.AddProject("library", Application(package, HostModel.None));
        tree.AddProject("app", Application(package, host, referencePackage: false).AddProjectReference("../library/library.csproj"));
        tree.AddProject("direct", Application(package, host));

        await tree.Build("app", Cancellation);
        await tree.Build("direct", Cancellation);

        // the application did receive the package: its assets are imported (it declares the template) and the assembly is copied to its output
        ( await tree.DeclaredTemplates("app", Cancellation) ).ShouldContain(HealthChecksJoint);
        tree.HasOutput("app", "Rocket.Surgery.LaunchPad.Hosting.dll").ShouldBeTrue();
        var transitive = await tree.Run("app", Cancellation);
        var direct = await tree.Run("direct", Cancellation);

        await Verify(
            new
            {
                Host = host,
                Transitive = new { Templates = tree.EmittedTemplates("app"), Output = transitive.Trim() },
                Direct = new { Templates = tree.EmittedTemplates("direct"), Output = direct.Trim() },
            }
        );
    }

    /// <summary>
    ///     A solution consumes its own packages by project reference, which imports no package assets, so the repository's
    ///     <c>Directory.Build.targets</c> imports the targets into every project (the samples and tests reference Hosting this way). The
    ///     tree is inside the repository so that file applies to it. A project with a project reference to Hosting gets the joint, and a
    ///     project that only references a library that references Hosting gets nothing. It runs on its own: all the projects build
    ///     <c>src/Hosting</c> in the repository.
    /// </summary>
    [Test]
    [NotInParallel("src/Hosting")]
    public async Task Should_Give_An_Application_With_A_Project_Reference_To_Hosting_The_HealthChecks_Joint_Through_The_Repository_Targets()
    {
        using var tree = ThrowawayTree.InsideTheRepository();
        tree.AddProject("library", AppProject.Central(HostModel.None).AddProjectReference(ThrowawayTree.HostingProject));
        tree.AddProject("direct", AppProject.Central(HostModel.GenericHost).AddPackage(AppProject.HealthChecks).AddProjectReference(ThrowawayTree.HostingProject));
        tree.AddProject("transitive", AppProject.Central(HostModel.GenericHost).AddPackage(AppProject.HealthChecks).AddProjectReference("../library/library.csproj"));

        await tree.Build("direct", Cancellation);
        await tree.Build("transitive", Cancellation);
        var direct = await tree.Run("direct", Cancellation);
        var transitive = await tree.Run("transitive", Cancellation);

        await Verify(
            new
            {
                Direct = new { Templates = tree.EmittedTemplates("direct"), Output = direct.Trim() },
                Transitive = new { Templates = tree.EmittedTemplates("transitive"), Output = transitive.Trim() },
                Library = new { Templates = tree.EmittedTemplates("library") },
            }
        );
    }

    /// <summary>
    ///     An application of <paramref name="host" /> in a tree outside the repository, with the packed package and what the host
    ///     needs.
    /// </summary>
    private static AppProject Application(PackedHosting package, HostModel host, bool healthChecks = true, bool referencePackage = true)
    {
        var application = AppProject.Standalone(host);
        if (referencePackage) application.AddPackage(package.PackageId, package.Version);

        // ASP.NET Core has health checks in its framework, and a library has no use for them
        if (healthChecks && host is HostModel.GenericHost or HostModel.WebAssembly) application.AddPackage(AppProject.HealthChecks);
        return application;
    }
}
