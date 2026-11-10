using System.Text.Json.Nodes;

using Joinery.Dovetail;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.AspNetCore.Joints;
using Rocket.Surgery.LaunchPad.AspNetCore.Validation;

using ZeroAlloc.Validation;

namespace ZeroAlloc.AspNetCore.Tests;

public class ZeroAllocOpenApiSchemaTransformerTests
{
    [Test]
    public async Task Describes_The_Validation_Attributes_In_The_Schema()
    {
        await using var app = await Start();
        using var client = app.GetTestClient();

        var document = JsonNode.Parse(await client.GetStringAsync(new Uri("/openapi/v1.json", UriKind.Relative), CancellationToken.None))!;
        var schema = document["components"]!["schemas"]![nameof(Rocket)]!;

        await Verify(schema.ToJsonString(new() { WriteIndented = true }));
    }

    [Test]
    public async Task Requires_The_Members_Of_The_Problem_Details()
    {
        await using var app = await Start();
        using var client = app.GetTestClient();

        var document = JsonNode.Parse(await client.GetStringAsync(new Uri("/openapi/v1.json", UriKind.Relative), CancellationToken.None))!;
        var schemas = document["components"]!["schemas"]!;

        Required(schemas["ProblemDetails"]).ShouldBe(["type", "title"]);
        Required(schemas["HttpValidationProblemDetails"]).ShouldBe(["type", "title", "errors"]);
        Required(schemas[nameof(ValidationFailureProblemDetails)]).ShouldBe(["type", "title", "errors"]);
    }

    private static string?[] Required(JsonNode? schema) => [.. schema!["required"]!.AsArray().Select(x => x!.GetValue<string>())];

    private static async Task<WebApplication> Start()
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddOpenApi();
        new ZeroAllocOpenApiJoint().Register(await Context(), builder.Services);
        var app = builder.Build();
        app.MapOpenApi();
        app.MapPost("/rockets", (Rocket rocket) => TypedResults.Ok(rocket)).ProducesProblem(500).ProducesValidationProblem();
        app.MapGet("/failure", () => TypedResults.UnprocessableEntity(new ValidationFailureProblemDetails()));
        await app.StartAsync();
        return app;
    }

    public class Rocket
    {
        [NotEmpty]
        [MinLength(10)]
        [MaxLength(30)]
        public string SerialNumber { get; set; } = "";

        [Validation.NotNull]
        public string? Name { get; set; }

        [InclusiveBetween(1, 10)]
        public int Stages { get; set; }

        [GreaterThan(0)]
        public double Thrust { get; set; }

        [LessThanOrEqualTo(100)]
        public int Crew { get; set; }

        [Matches("^[A-Z]+$")]
        public string Code { get; set; } = "";

        [EmailAddress]
        public string Contact { get; set; } = "";

        [NotEmpty]
        public List<string> Tags { get; set; } = [];

        public string? Notes { get; set; }
    }

    private static async Task<IDovetailContext> Context() =>
        await DovetailContext.FromAsync(DovetailContextBuilder.Create([], new Dictionary<object, object>(), []));
}
