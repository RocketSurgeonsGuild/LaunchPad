using Hosting.Tests.Scaffolding;

namespace Hosting.Tests;

/// <summary>
///     What <c>Rocket.Surgery.LaunchPad.Hosting</c> is as a package: what it depends on, and the build assets that carry its templates.
/// </summary>
public class HostingPackageTests
{
    /// <summary>
    ///     The package does not carry the dependencies of the joints that move into it later (Serilog and its sinks, OpenTelemetry, the
    ///     health checks): those are the application's own references, and referencing them is what turns a template on.
    /// </summary>
    [Test]
    public async Task Should_Depend_On_Dovetail_And_The_Hosting_Abstractions_Only()
    {
        var package = await PackedHosting.Pack();

        await Verify(package.Dependencies);
    }

    /// <summary>
    ///     The templates are declared in <c>build/</c>, which NuGet imports into the projects that reference the package, and in
    ///     <c>buildTransitive/</c>, which it imports into every project that receives the package. A project that receives the package
    ///     transitively still declares the templates, and the direct-reference gate is what keeps them out of it.
    /// </summary>
    [Test]
    public async Task Should_Ship_Its_Templates_As_Build_Assets()
    {
        var package = await PackedHosting.Pack();

        await Verify(package.Files.Where(z => z.StartsWith("build", StringComparison.Ordinal)));
    }
}
