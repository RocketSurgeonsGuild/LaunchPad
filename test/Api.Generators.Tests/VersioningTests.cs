namespace Api.Generators.Tests;

/// <summary>Versions on hosts and operations are mapped with Asp.Versioning when the server references it, and reported when it doesn't.</summary>
public class VersioningTests() : GeneratorTest()
{
    private const string Usings = "using Rocket.Surgery.LaunchPad.Api;\nusing Sample.Contracts.Rockets;\n";

    private const string Endpoints = Usings + """
        [OperationHost(Version = "1.0")]
        [Operation<GetRocket.Request>(Version = "2.0")]
        [Operation<CreateRocket.Request>]
        public static partial class RocketsEndpoints;
        """;

    private const string Controller = Usings + """
        [OperationHost(Version = "1.0")]
        [Operation<GetRocket.Request>(Version = "2.0")]
        [Operation<CreateRocket.Request>]
        public partial class RocketsController;
        """;

    private static readonly System.Reflection.Assembly[] Versioning =
        [typeof(Asp.Versioning.ApiVersion).Assembly, typeof(Asp.Versioning.Builder.ApiVersionSet).Assembly];

    private static readonly System.Reflection.Assembly[] MvcVersioning = [.. Versioning, typeof(Asp.Versioning.ApiVersioningApplicationModelProvider).Assembly];

    [Test]
    public async Task Should_Map_Versions_On_Minimal_Api_Endpoints()
    {
        var contracts = await CompileContracts(Mediator.MediatR, Contracts.For(Mediator.MediatR));
        var result = await Server(Mediator.MediatR, contracts).AddReferences(Versioning).AddSources(Endpoints).Build().GenerateAsync(TestContext.CancellationToken);

        AssertCompiles(result);
        await Verify(result);
    }

    [Test]
    public async Task Should_Map_Versions_On_Controllers()
    {
        var contracts = await CompileContracts(Mediator.MediatR, Contracts.For(Mediator.MediatR));
        var result = await Controllers(Mediator.MediatR, contracts).AddReferences(MvcVersioning).AddSources(Controller).Build().GenerateAsync(TestContext.CancellationToken);

        AssertCompiles(result);
        await Verify(result);
    }

    [Test]
    public async Task Should_Report_Versions_Without_Asp_Versioning()
    {
        var contracts = await CompileContracts(Mediator.MediatR, Contracts.For(Mediator.MediatR));
        var result = await Server(Mediator.MediatR, contracts).AddSources(Endpoints).Build().GenerateAsync(TestContext.CancellationToken);

        AssertCompiles(result);
        DiagnosticIds(result).ShouldContain("LPAD1024");
    }

    [Test]
    public async Task Should_Report_Controller_Versions_Without_Asp_Versioning_Mvc()
    {
        // Asp.Versioning.Http brings in the attributes, but MVC only applies them with Asp.Versioning.Mvc.
        var contracts = await CompileContracts(Mediator.MediatR, Contracts.For(Mediator.MediatR));
        var result = await Controllers(Mediator.MediatR, contracts).AddReferences(Versioning).AddSources(Controller).Build().GenerateAsync(TestContext.CancellationToken);

        AssertCompiles(result);
        DiagnosticIds(result).ShouldContain("LPAD1024");
    }
}
