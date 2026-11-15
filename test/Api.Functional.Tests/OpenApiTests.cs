extern alias zeroallocserver;
using System.Text.Json;
using Microsoft.AspNetCore.TestHost;
using Launches = zeroallocserver::Api.Functional.ZeroAlloc.Server;

namespace Api.Functional.Tests;

/// <summary>
///     The OpenAPI document a running server builds with the generated operation transformers: a schema per union error case keyed on
///     <c>case</c>, the 201 <c>Location</c> header, 401 and 403 for an operation with a policy, and the request's ZeroAlloc.Validation rules.
///     The application registers no transformer: each endpoint carries its own.
/// </summary>
public class OpenApiTests
{
    [Test]
    [Arguments(false)]
    [Arguments(true)]
    public async Task Should_Describe_Each_Union_Case(bool controllers, CancellationToken cancellationToken)
    {
        var document = await Document(controllers, cancellationToken);
        var responses = document.GetProperty("paths").GetProperty("/api/launches/{id}").GetProperty("post").GetProperty("responses");

        var notFound = Schema(responses, "404");
        await notFound.GetProperty("discriminator").GetProperty("propertyName").GetString().Should().BeEqualTo("case");
        await notFound.GetProperty("discriminator").GetProperty("mapping").GetProperty("RocketNotFound").GetString().Should().BeEqualTo("#/components/schemas/RocketNotFoundProblem");
        await notFound.GetProperty("oneOf").EnumerateArray().Select(s => s.GetProperty("$ref").GetString()!).Should().BeEquivalentTo(["#/components/schemas/RocketNotFoundProblem"]);
        await Schema(responses, "422").GetProperty("oneOf").EnumerateArray().Select(s => s.GetProperty("$ref").GetString()!).Should().BeEquivalentTo(["#/components/schemas/NotReadyProblem"]);

        // The case's schema: the problem details, its name in `case` and its members.
        var schemas = document.GetProperty("components").GetProperty("schemas");
        var allOf = schemas.GetProperty("RocketNotFoundProblem").GetProperty("allOf").EnumerateArray().ToArray();
        await allOf[0].GetProperty("$ref").GetString().Should().BeEqualTo("#/components/schemas/ProblemDetails");
        await allOf[1].GetProperty("required").EnumerateArray().Select(r => r.GetString()!).Should().BeEquivalentTo(["case"]);
        await allOf[1].GetProperty("properties").GetProperty("case").GetProperty("enum").EnumerateArray().Select(e => e.GetString()!).Should().BeEquivalentTo(["RocketNotFound"]);
        await allOf[1].GetProperty("properties").GetProperty("id").GetProperty("format").GetString().Should().BeEqualTo("uuid");
        var notReady = schemas.GetProperty("NotReadyProblem").GetProperty("allOf").EnumerateArray().ElementAt(1).GetProperty("properties");
        await notReady.GetProperty("reason").GetProperty("type").GetString().Should().BeEqualTo("string");
    }

    [Test]
    [Arguments(false)]
    [Arguments(true)]
    public async Task Should_Describe_The_Created_Location(bool controllers, CancellationToken cancellationToken)
    {
        var document = await Document(controllers, cancellationToken);
        var created = document.GetProperty("paths").GetProperty("/api/rockets").GetProperty("post").GetProperty("responses").GetProperty("201");

        await created.GetProperty("headers").GetProperty("Location").GetProperty("schema").GetProperty("format").GetString().Should().BeEqualTo("uri");
    }

    [Test]
    [Arguments(false)]
    [Arguments(true)]
    public async Task Should_Describe_Authorization_And_Validation_Rules(bool controllers, CancellationToken cancellationToken)
    {
        var document = await Document(controllers, cancellationToken);
        var operation = document.GetProperty("paths").GetProperty("/api/launches/{id}/schedule").GetProperty("post");
        var responses = operation.GetProperty("responses");

        await responses.EnumerateObject().Select(r => r.Name).Should().BeEquivalentTo(["200", "400", "401", "403", "404", "422"]);
        await responses.GetProperty("401").GetProperty("headers").TryGetProperty("WWW-Authenticate", out _).Should().BeTrue();

        // The body is the request's schema in the components, which carries the rules.
        var reference = operation.GetProperty("requestBody").GetProperty("content").GetProperty("application/json").GetProperty("schema").GetProperty("$ref").GetString()!;
        var body = document.GetProperty("components").GetProperty("schemas").GetProperty(reference.Split('/')[^1]);
        await body.GetProperty("required").EnumerateArray().Select(r => r.GetString()!).Should().Contain("window");
        var properties = body.GetProperty("properties");
        await properties.GetProperty("window").GetProperty("minLength").GetInt32().Should().BeEqualTo(1);
        await properties.GetProperty("window").GetProperty("maxLength").GetInt32().Should().BeEqualTo(40);
        await properties.GetProperty("window").GetProperty("pattern").GetString().Should().BeEqualTo("^[A-Za-z ]+$");
        await properties.GetProperty("crew").GetProperty("minimum").GetDouble().Should().BeEqualTo(1);
        await properties.GetProperty("crew").GetProperty("maximum").GetDouble().Should().BeEqualTo(10);
        await properties.GetProperty("contact").GetProperty("format").GetString().Should().BeEqualTo("email");
    }

    private static JsonElement Schema(JsonElement responses, string status) =>
        responses.GetProperty(status).GetProperty("content").GetProperty("application/problem+json").GetProperty("schema");

    private static async Task<JsonElement> Document(bool controllers, CancellationToken cancellationToken)
    {
        await using var server = await Launches.ServerApp.StartAsync(controllers, versioned: false, unions: true);
        using var http = server.GetTestClient();
        return ( await http.GetFromJsonAsync<JsonElement>("openapi/v1.json", cancellationToken) ).Clone();
    }
}
