using System.Net;
using Sample.Classic.Restful.Tests.Helpers;

namespace Sample.Classic.Restful.Tests;

public class ClassicFoundationTests(ITestOutputHelper testOutputHelper, TestWebAppFixture fixture)
    : WebAppFixtureTest<TestWebAppFixture>(testOutputHelper, fixture)
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
        await VerifyJson(response.Content.ReadAsStreamAsync());
    }
}
