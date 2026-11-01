using Microsoft.Build.Utilities.ProjectCreation;

using Sdk.Tests.Scaffolding;

namespace Sdk.Tests;

/// <summary>
///     A project that references Newtonsoft.Json itself gets <c>Rocket.Surgery.LaunchPad.Foundation.NewtonsoftJson</c>, and the Newtonsoft.Json
///     variant of each LaunchPad integration whose partner it also references. Like every detection of the SDK, it looks at the project's
///     direct package references only.
/// </summary>
/// <param name="targetFramework">The target framework of the throwaway projects.</param>
[TargetFrameworks]
public class SdkNewtonsoftJsonTests(string targetFramework)
{
    private static CancellationToken Cancellation => TestContext.Current!.Execution.CancellationToken;

    /// <summary>
    ///     An application that references Newtonsoft.Json gets the Newtonsoft.Json converters of LaunchPad's foundation, and nothing for the
    ///     partners it does not reference.
    /// </summary>
    [Test]
    public async Task Should_Add_Foundation_NewtonsoftJson_For_A_Direct_Newtonsoft_Json_Reference()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.GenericHost, App(HostModel.GenericHost).ItemPackageReference(ThirdParty.NewtonsoftJson, ThirdParty.NewtonsoftJsonVersion));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     ASP.NET Core is the partner of LaunchPad's ASP.NET Core integration: an ASP.NET Core application that references Newtonsoft.Json
    ///     also gets its MVC Newtonsoft.Json formatters.
    /// </summary>
    [Test]
    public async Task Should_Add_AspNetCore_NewtonsoftJson_To_An_AspNetCore_Application()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.AspNetCore, App(HostModel.AspNetCore).ItemPackageReference(ThirdParty.NewtonsoftJson, ThirdParty.NewtonsoftJsonVersion));

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    /// <summary>
    ///     Mapperly is the partner of LaunchPad's mapping: a project that references both gets the Newtonsoft.Json mappers. A library is enough,
    ///     as the detection does not depend on the host model.
    /// </summary>
    [Test]
    public async Task Should_Add_Mapping_NewtonsoftJson_To_A_Project_That_References_Mapperly()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject(
            "library",
            HostModel.None,
            App(HostModel.None)
               .ItemPackageReference(ThirdParty.NewtonsoftJson, ThirdParty.NewtonsoftJsonVersion)
               .ItemPackageReference(ThirdParty.Mapperly, ThirdParty.MapperlyVersion)
        );

        await Verify(await tree.Evaluate("library", Cancellation));
    }

    /// <summary>
    ///     An application that only receives Newtonsoft.Json through a library it references gets nothing: the library chose Newtonsoft.Json,
    ///     not the application. The library references it itself, so it gets the foundation package.
    /// </summary>
    [Test]
    public async Task Should_Not_Add_Anything_For_A_Transitive_Newtonsoft_Json_Reference()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("library", HostModel.None, App(HostModel.None).ItemPackageReference(ThirdParty.NewtonsoftJson, ThirdParty.NewtonsoftJsonVersion));
        tree.AddProject("app", HostModel.AspNetCore, App(HostModel.AspNetCore).ItemProjectReference("../library/library.csproj"));

        await Verify(new { Application = await tree.Evaluate("app", Cancellation), Library = await tree.Evaluate("library", Cancellation) });
    }

    /// <summary>
    ///     <c>LaunchPadEnableNewtonsoftJson</c> set to <c>false</c> keeps every Newtonsoft.Json package out, the partner variants included.
    /// </summary>
    [Test]
    public async Task Should_Not_Add_Anything_When_Opted_Out()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject(
            "app",
            HostModel.AspNetCore,
            App(HostModel.AspNetCore)
               .Property("LaunchPadEnableNewtonsoftJson", "false")
               .ItemPackageReference(ThirdParty.NewtonsoftJson, ThirdParty.NewtonsoftJsonVersion)
               .ItemPackageReference(ThirdParty.Mapperly, ThirdParty.MapperlyVersion)
        );

        await Verify(await tree.Evaluate("app", Cancellation));
    }

    private ProjectCreator App(HostModel host) => HostModelShape.Project(host, targetFramework);
}
