using System.Text.Json;
using System.Text.Json.Nodes;

using Hosting.Tests.Scaffolding;

using Microsoft.Build.Utilities.ProjectCreation;

using Sdk.Tests.Scaffolding;

namespace Sdk.Tests;

/// <summary>
///     Every LaunchPad SDK chains Chirograph, the MSBuild SDK that generates typed HTTP clients from the project's <c>OpenApiClient</c>
///     items. An application gets the client and, through Dovetail, a joint that registers it, with no other SDK entry.
/// </summary>
/// <param name="targetFramework">The target framework of the throwaway projects.</param>
[TargetFrameworks]
public class SdkChirographTests(string targetFramework)
{
    private static CancellationToken Cancellation => TestContext.Current!.Execution.CancellationToken;

    /// <summary>
    ///     The smoke test: a Kiota client generated from an OpenAPI document in the project is resolved from a host built with
    ///     <c>ConfigureDovetail()</c>, which only Chirograph's joint can have registered it in.
    /// </summary>
    [Test]
    public async Task Should_Resolve_A_Kiota_Client_From_A_File_With_ConfigureDovetail()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.GenericHost, KiotaClient(HostModelShape.Project(HostModel.GenericHost, targetFramework)));
        await File.WriteAllTextAsync(Path.Combine(tree.Directory, "app", "rockets.json"), RocketsDocument, Cancellation);
        await File.WriteAllTextAsync(Path.Combine(tree.Directory, "app", "Program.cs"), ResolveClient, Cancellation);

        await tree.Build("app", Cancellation);
        var output = await tree.Run("app", Cancellation);

        await output.Trim().Should().BeEqualTo($"Client:{ClientType}");
    }

    /// <summary>
    ///     A project that applies Chirograph itself, next to the LaunchPad SDK and at another version, gets it once, at its own version:
    ///     each client is declared once, so it is generated once.
    /// </summary>
    [Test]
    public async Task Should_Not_Chain_Chirograph_Into_A_Project_That_Applies_It_Itself()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.GenericHost, KiotaClient(HostModelShape.Project(HostModel.GenericHost, targetFramework).Sdk("Chirograph", OtherChirographVersion)));

        var evaluation = await EvaluateChirograph(tree, "app");

        await evaluation.Version.Should().BeEqualTo(OtherChirographVersion);
        await evaluation.Clients.Should().BeEqualTo(1);
    }

    /// <summary>
    ///     <c>LaunchPadEnableChirograph</c> set to <c>false</c> leaves Chirograph out: nothing reads the <c>OpenApiClient</c> items.
    /// </summary>
    [Test]
    public async Task Should_Not_Chain_Chirograph_When_Opted_Out()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.GenericHost, KiotaClient(HostModelShape.Project(HostModel.GenericHost, targetFramework).Property("LaunchPadEnableChirograph", "false")));

        var evaluation = await EvaluateChirograph(tree, "app");

        await evaluation.Version.Should().BeEqualTo("");
        await evaluation.Clients.Should().BeEqualTo(0);
    }

    /// <summary>
    ///     The version of Chirograph the LaunchPad SDKs chain (Directory.Packages.props) is the one the repository's own projects that
    ///     apply <c>&lt;Sdk Name="Chirograph" /&gt;</c> resolve (msbuild-sdks in global.json), so the tests and the SDK use the same Chirograph.
    /// </summary>
    [Test]
    public async Task Should_Pin_The_Same_Chirograph_In_GlobalJson()
    {
        var globalJson = JsonNode.Parse(await File.ReadAllTextAsync(Path.Combine(Repository.Root, "global.json"), Cancellation))!;

        await ( (string?)globalJson["msbuild-sdks"]?["Chirograph"] ).Should().BeEqualTo(PackedSdk.ChirographVersion);
    }

    // a released Chirograph other than the one the repository pins
    private static string OtherChirographVersion => PackedSdk.ChirographVersion == "0.1.1" ? "0.1.0" : "0.1.1";

    private const string ClientType = $"{HostModelShape.RootNamespace}.Clients.RocketsClient";

    private static ProjectCreator KiotaClient(ProjectCreator project) =>
        project.ItemInclude(
            "OpenApiClient",
            "rockets.json",
            metadata: new Dictionary<string, string?>
            {
                ["Generator"] = "Kiota",
                ["ClientName"] = "RocketsClient",
                ["Namespace"] = $"{HostModelShape.RootNamespace}.Clients",
            }
        );

    /// <summary>
    ///     The version of Chirograph a project evaluates with (empty when nothing imported it) and how many clients it declares to generate.
    /// </summary>
    private static async Task<(string Version, int Clients)> EvaluateChirograph(SdkTree tree, string name)
    {
        var output = await DotnetCli.Run(
            $"msbuild \"{Path.Combine(tree.Directory, name, $"{name}.csproj")}\" -nologo -getProperty:ChirographVersion -getItem:_ChirographOpenApiClient",
            tree.Directory,
            null,
            Cancellation
        );
        // -getProperty writes JSON after anything the SDK resolver logs while it restores the SDKs
        using var json = JsonDocument.Parse(output[output.IndexOf('{', StringComparison.Ordinal)..]);
        var version = json.RootElement.GetProperty("Properties").GetProperty("ChirographVersion").GetString() ?? "";
        var clients = json.RootElement.GetProperty("Items").TryGetProperty("_ChirographOpenApiClient", out var items) ? items.GetArrayLength() : 0;
        return (version, clients);
    }

    private const string RocketsDocument = """
        {
          "openapi": "3.0.1",
          "info": { "title": "Rockets", "version": "v1" },
          "servers": [ { "url": "https://rockets.example/" } ],
          "paths": {
            "/rockets": {
              "get": {
                "operationId": "ListRockets",
                "responses": {
                  "200": {
                    "description": "OK",
                    "content": { "application/json": { "schema": { "type": "array", "items": { "$ref": "#/components/schemas/Rocket" } } } }
                  }
                }
              }
            }
          },
          "components": {
            "schemas": {
              "Rocket": { "type": "object", "properties": { "id": { "type": "string" }, "name": { "type": "string" } } }
            }
          }
        }
        """;

    // nothing registers the client but the joint Chirograph declares and Dovetail writes
    private const string ResolveClient = $$"""
        using {{HostModelShape.RootNamespace}};

        var host = await Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(args).ConfigureDovetail();
        var client = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<{{ClientType}}>(host.Services);
        System.Console.WriteLine($"Client:{client.GetType().FullName}");
        """;
}
