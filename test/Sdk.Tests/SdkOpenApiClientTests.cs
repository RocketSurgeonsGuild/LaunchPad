using Sdk.Tests.Scaffolding;

namespace Sdk.Tests;

/// <summary>
///     An <c>OpenApiClient</c> item generates an HTTP client from an OpenAPI document with Kiota or Refitter, and a joint that registers it
///     with <c>IHttpClientFactory</c>, its base address read from <c>Clients:&lt;ClientName&gt;:BaseAddress</c>. The document comes from a
///     file, a server project that writes it when it builds, or a URL. Each throwaway application resolves its client from the host
///     <c>ConfigureDovetail</c> builds and calls <see cref="RocketsApi" />, which the test serves on the loopback interface.
/// </summary>
/// <param name="targetFramework">The target framework of the throwaway projects.</param>
[TargetFrameworks]
public class SdkOpenApiClientTests(string targetFramework)
{
    private static CancellationToken Cancellation => TestContext.Current!.Execution.CancellationToken;

    /// <summary>
    ///     Each generator generates a client from each kind of source, and the joint registers it with the configured base address.
    /// </summary>
    /// <param name="generator">The generator.</param>
    /// <param name="source">Where the document comes from.</param>
    [Test]
    [Arguments(Generator.Kiota, Source.File)]
    [Arguments(Generator.Kiota, Source.Project)]
    [Arguments(Generator.Kiota, Source.Url)]
    [Arguments(Generator.Refitter, Source.File)]
    [Arguments(Generator.Refitter, Source.Project)]
    [Arguments(Generator.Refitter, Source.Url)]
    public async Task Should_Generate_A_Registered_Client(Generator generator, Source source)
    {
        await using var api = RocketsApi.Start();
        using var tree = await Tree(generator, source, api);

        await tree.Build("app", Cancellation);

        // one snapshot for both target frameworks
        await Verify(await tree.Run("app", Cancellation, environment: Environment(api))).UseTextForParameters($"{generator}_{source}");
    }

    /// <summary>
    ///     A client is only generated again when its document changes: not on the next build, and not when the document is written again
    ///     with the same content.
    /// </summary>
    /// <param name="generator">The generator.</param>
    [Test]
    [Arguments(Generator.Kiota)]
    [Arguments(Generator.Refitter)]
    public async Task Should_Only_Generate_A_Client_Again_When_Its_Document_Changes(Generator generator)
    {
        await using var api = RocketsApi.Start();
        using var tree = await Tree(generator, Source.File, api);
        var document = Path.Combine(tree.Directory, "app", "rockets.json");
        var generating = $"Generating the OpenAPI client RocketsClient with {generator}";

        var first = await tree.Build("app", Cancellation);
        var unchanged = await tree.Build("app", Cancellation);
        await File.WriteAllTextAsync(document, RocketsApi.Document, Cancellation);
        File.SetLastWriteTimeUtc(document, DateTime.UtcNow.AddMinutes(1));
        var rewritten = await tree.Build("app", Cancellation);
        await File.WriteAllTextAsync(document, RocketsApi.Document.Replace("\"serialNumber\"", "\"name\": { \"type\": \"string\" }, \"serialNumber\"", StringComparison.Ordinal), Cancellation);
        var changed = await tree.Build("app", Cancellation);

        await first.Should().Contain(generating);
        await unchanged.Should().NotContain(generating);
        await rewritten.Should().NotContain(generating);
        await changed.Should().Contain(generating);
    }

    /// <summary>
    ///     A document from a URL is downloaded on every build, and a client is only generated again when it changes. When the URL cannot be
    ///     reached, the build uses the document it downloaded before, with a warning.
    /// </summary>
    [Test]
    public async Task Should_Use_The_Last_Download_When_The_Url_Cannot_Be_Reached()
    {
        var api = RocketsApi.Start();
        using var tree = await Tree(Generator.Kiota, Source.Url, api);
        const string generating = "Generating the OpenAPI client RocketsClient with Kiota";

        string first, again;
        try
        {
            first = await tree.Build("app", Cancellation);
            again = await tree.Build("app", Cancellation);
            await api.DocumentRequests.Should().BeEqualTo(2);
        }
        finally
        {
            await api.DisposeAsync();
        }

        var offline = await tree.Build("app", Cancellation);

        await first.Should().Contain(generating);
        await again.Should().NotContain(generating);
        await offline.Should().Contain("LPSDK0104");
        await offline.Should().NotContain(generating);
    }

    /// <summary>
    ///     A client without a generator, or without a name, fails the build with what to set.
    /// </summary>
    /// <param name="generator">The generator the client declares, if any.</param>
    /// <param name="clientName">The name the client declares, if any.</param>
    /// <param name="code">The error.</param>
    [Test]
    [Arguments(null, "RocketsClient", "LPSDK0100")]
    [Arguments("NSwag", "RocketsClient", "LPSDK0100")]
    [Arguments("Kiota", null, "LPSDK0101")]
    [Arguments("Kiota", "Rockets.Client", "LPSDK0101")]
    public async Task Should_Fail_When_A_Client_Has_No_Generator_Or_Name(string? generator, string? clientName, string code)
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject(
            "app",
            HostModel.GenericHost,
            HostModelShape
               .Project(HostModel.GenericHost, targetFramework)
               .ItemInclude("OpenApiClient", "rockets.json", metadata: new Dictionary<string, string?> { ["Generator"] = generator, ["ClientName"] = clientName, })
        );
        await File.WriteAllTextAsync(Path.Combine(tree.Directory, "app", "rockets.json"), RocketsApi.Document, Cancellation);

        string failure;
        try
        {
            await tree.Build("app", Cancellation);
            failure = "the build succeeded";
        }
        catch (InvalidOperationException e)
        {
            failure = e.Message;
        }

        await failure.Should().Contain($"error {code}");
    }

    /// <summary>
    ///     The generators.
    /// </summary>
    public enum Generator
    {
        /// <summary>Kiota: a client class, <c>RocketsClient</c>.</summary>
        Kiota,

        /// <summary>Refitter: a Refit interface, <c>IRocketsClient</c>.</summary>
        Refitter,
    }

    /// <summary>
    ///     Where a document comes from.
    /// </summary>
    public enum Source
    {
        /// <summary>A file in the project.</summary>
        File,

        /// <summary>A server project, which writes its document when it builds.</summary>
        Project,

        /// <summary>A URL.</summary>
        Url,
    }

    private static Dictionary<string, string?> Environment(RocketsApi api) => new() { ["Clients__RocketsClient__BaseAddress"] = api.BaseAddress.ToString(), };

    private async Task<SdkTree> Tree(Generator generator, Source source, RocketsApi api)
    {
        var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        var include = source switch
        {
            Source.File => "rockets.json",
            Source.Project => "../server/server.csproj",
            Source.Url => api.DocumentAddress.ToString(),
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null),
        };
        if (source == Source.Project) AddServer(tree);

        tree.AddProject(
            "app",
            HostModel.GenericHost,
            HostModelShape
               .Project(HostModel.GenericHost, targetFramework)
               .ItemInclude(
                    "OpenApiClient",
                    include,
                    metadata: new Dictionary<string, string?>
                    {
                        ["Generator"] = generator.ToString(),
                        ["ClientName"] = "RocketsClient",
                        ["Namespace"] = "Throwaway.Clients",
                    }
                )
        );
        await File.WriteAllTextAsync(Path.Combine(tree.Directory, "app", "rockets.json"), RocketsApi.Document, Cancellation);
        await File.WriteAllTextAsync(Path.Combine(tree.Directory, "app", "Program.cs"), generator == Generator.Kiota ? KiotaApp : RefitterApp, Cancellation);
        return tree;
    }

    // An ASP.NET Core server whose document, written at build by Microsoft.Extensions.ApiDescription.Server, describes the API RocketsApi
    // serves. It registers the document the way a LaunchPad application does, through LaunchPad's OpenApiJoint: an AddOpenApi() call of its
    // own would have ASP.NET Core's XML comment generator read Joinery.Dovetail's documentation, which it fails on.
    private void AddServer(SdkTree tree)
    {
        tree.AddProject(
            "server",
            HostModel.AspNetCore,
            HostModelShape
               .Project(HostModel.AspNetCore, targetFramework)
               .ItemPackageReference("Microsoft.AspNetCore.OpenApi", Repository.PackageVersion("Microsoft.AspNetCore.OpenApi", targetFramework))
               .ItemPackageReference(
                    "Microsoft.Extensions.ApiDescription.Server",
                    Repository.PackageVersion("Microsoft.Extensions.ApiDescription.Server", targetFramework),
                    privateAssets: "all"
                )
        );
        File.WriteAllText(
            Path.Combine(tree.Directory, "server", "Program.cs"),
            """
            using System;
            using Microsoft.AspNetCore.Builder;
            using Microsoft.AspNetCore.Http;
            using Throwaway;

            // LaunchPad's OpenApiJoint registers the document
            var app = await WebApplication.CreateBuilder(args).ConfigureDovetail();
            app.MapGet("/rockets/{id}", (Guid id) => new Throwaway.Server.Rocket(id, "FALCON-9")).WithName("GetRocket");
            app.MapOpenApi();
            await app.RunAsync();

            namespace Throwaway.Server
            {
                public sealed record Rocket(Guid Id, string SerialNumber);
            }
            """
        );
    }

    private const string KiotaApp = $$"""
        using System;
        using Microsoft.Extensions.DependencyInjection;
        using Microsoft.Extensions.Logging;
        using Throwaway;
        using Throwaway.Clients;

        var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(args);
        builder.Logging.ClearProviders();
        using var host = await builder.ConfigureDovetail();
        var client = host.Services.GetRequiredService<RocketsClient>();
        var rocket = await client.Rockets[Guid.Parse("{{RocketsApi.RocketId}}")].GetAsync();
        Console.WriteLine($"{client.GetType().Name}: {rocket!.Id} {rocket.SerialNumber}");
        """;

    private const string RefitterApp = $$"""
        using System;
        using Microsoft.Extensions.DependencyInjection;
        using Microsoft.Extensions.Logging;
        using Throwaway;
        using Throwaway.Clients;

        var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(args);
        builder.Logging.ClearProviders();
        using var host = await builder.ConfigureDovetail();
        var client = host.Services.GetRequiredService<IRocketsClient>();
        var rocket = await client.GetRocket(Guid.Parse("{{RocketsApi.RocketId}}"));
        Console.WriteLine($"{nameof(IRocketsClient)}: {rocket.Id} {rocket.SerialNumber}");
        """;
}
