using Sdk.Tests.Scaffolding;

namespace Sdk.Tests;

/// <summary>
///     What <c>Rocket.Surgery.LaunchPad.Sdk</c> is as a package: an MSBuild SDK, and the versions it adds packages at, which are written
///     when it is packed.
/// </summary>
public class SdkPackageTests
{
    /// <summary>
    ///     The SDK is MSBuild files only.
    /// </summary>
    [Test]
    public async Task Should_Ship_Only_Msbuild_Files()
    {
        var packages = await PackedSdk.Pack();

        await Verify(packages.SdkFiles);
    }

    /// <summary>
    ///     The LaunchPad packages are added at the version of the SDK, and Dovetail's SDK is chained at the version the repository pins. Each
    ///     version is a property an application can set first.
    /// </summary>
    [Test]
    public async Task Should_Generate_The_Package_Versions_When_Packed()
    {
        var packages = await PackedSdk.Pack();

        await Verify(packages.Scrub(packages.PackagesProps), "xml");
    }
}
