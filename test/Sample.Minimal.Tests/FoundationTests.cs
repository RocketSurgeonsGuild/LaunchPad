using System.Net;

using Sample.Minimal.Tests.Helpers;

namespace Sample.Minimal.Tests;

public class FoundationTests : WebAppFixtureTest
{
    [Test]
    public async Task Starts()
    {
        var response = await AlbaHost.Server.CreateClient().GetAsync("/", TestContext.CancellationToken);
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    // the document the LaunchPad OpenAPI transformers shape: operation ids, parameter names, status codes, media types and schema ids
    [Test]
    public async Task OpenApiDocument()
    {
        var response = await AlbaHost.Server.CreateClient().GetAsync("/openapi/v1.json", TestContext.CancellationToken);
        var document = await response.Content.ReadAsStringAsync(TestContext.CancellationToken);
        response.StatusCode.ShouldBe(HttpStatusCode.OK, document);
        // net10.0 serves OpenAPI 3.1 and net11.0 OpenAPI 3.2
        await VerifyJson(document).UniqueForTargetFrameworkAndVersion();
    }
}
