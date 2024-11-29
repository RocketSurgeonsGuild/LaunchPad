using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sample.Minimal.Tests.Helpers;

namespace Sample.Restful.Tests;

public class FoundationTests(ITestOutputHelper testOutputHelper, TestWebAppFixture factory) : WebAppFixtureTest<TestWebAppFixture>(testOutputHelper, factory)
{
    [Fact]
    public async Task Starts()
    {
        var response = await AlbaHost.Server.CreateClient().GetAsync("/");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task OpenApiDocument()
    {
        var response = await AlbaHost.Server.CreateClient().GetAsync("/openapi/v1.json");
        var document = await response.Content.ReadAsStringAsync();
        await Verify(document, extension: "json");
    }
}
