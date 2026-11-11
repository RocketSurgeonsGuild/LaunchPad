using Sdk.Tests.Scaffolding;

namespace Sdk.Tests;

/// <summary>
///     Before .NET 11, a project that gets <c>Rocket.Surgery.LaunchPad.Foundation</c> gets Polyfill too, with embedded types, so it can declare
///     the portable unions of docs/design/union-types.md. On .NET 11 the BCL has the union types and nothing is added.
/// </summary>
/// <param name="targetFramework">The target framework of the throwaway projects.</param>
[TargetFrameworks]
public class SdkPolyfillTests(string targetFramework)
{
    private static CancellationToken Cancellation => TestContext.Current!.Execution.CancellationToken;

    private bool BeforeNet11 => targetFramework == "net10.0";

    /// <summary>
    ///     An ASP.NET Core application gets Rocket.Surgery.LaunchPad.AspNetCore, which brings Foundation.
    /// </summary>
    [Test]
    public async Task Should_Add_Polyfill_Before_Net11_When_The_Project_Gets_Foundation()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.AspNetCore, HostModelShape.Project(HostModel.AspNetCore, targetFramework));

        var (polyfill, embedded) = await tree.EvaluatePolyfill("app", Cancellation);

        polyfill.ShouldBe(BeforeNet11);
        embedded.ShouldBe(BeforeNet11 ? "true" : null);
    }

    /// <summary>
    ///     A library that references Foundation itself gets Polyfill as well.
    /// </summary>
    [Test]
    public async Task Should_Add_Polyfill_Before_Net11_For_A_Direct_Foundation_Reference()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("library", HostModel.None, HostModelShape.Project(HostModel.None, targetFramework).ItemPackageReference("Rocket.Surgery.LaunchPad.Foundation"));

        ( await tree.EvaluatePolyfill("library", Cancellation) ).Polyfill.ShouldBe(BeforeNet11);
    }

    /// <summary>
    ///     A generic host gets only Rocket.Surgery.LaunchPad.Hosting, which doesn't bring Foundation.
    /// </summary>
    [Test]
    public async Task Should_Not_Add_Polyfill_Without_Foundation()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.GenericHost, HostModelShape.Project(HostModel.GenericHost, targetFramework));

        ( await tree.EvaluatePolyfill("app", Cancellation) ).Polyfill.ShouldBeFalse();
    }

    /// <summary>
    ///     <c>LaunchPadEnablePolyfill</c> set to <c>false</c>, or a direct reference to PolySharp, leaves Polyfill out.
    /// </summary>
    /// <param name="optOut">How the project opts out.</param>
    [Test]
    [Arguments("LaunchPadEnablePolyfill")]
    [Arguments("PolySharp")]
    public async Task Should_Not_Add_Polyfill_When_Opted_Out(string optOut)
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        var project = HostModelShape.Project(HostModel.AspNetCore, targetFramework);
        project = optOut == "PolySharp" ? project.ItemPackageReference("PolySharp") : project.Property("LaunchPadEnablePolyfill", "false");
        tree.AddProject("app", HostModel.AspNetCore, project);

        ( await tree.EvaluatePolyfill("app", Cancellation) ).Polyfill.ShouldBeFalse();
    }

    /// <summary>
    ///     A project that asks for Polyfill's public types keeps them: Polyfill allows only one of PolyPublic and PolyUseEmbeddedAttribute.
    /// </summary>
    [Test]
    public async Task Should_Not_Embed_When_The_Project_Asks_For_Public_Polyfills()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.AspNetCore, HostModelShape.Project(HostModel.AspNetCore, targetFramework).Property("PolyPublic", "true"));

        ( await tree.EvaluatePolyfill("app", Cancellation) ).PolyUseEmbeddedAttribute.ShouldBeNull();
    }
}
