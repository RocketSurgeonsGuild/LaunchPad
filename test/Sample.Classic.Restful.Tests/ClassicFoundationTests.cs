using System.Net;

using Sample.Classic.Restful.Tests.Helpers;

namespace Sample.Classic.Restful.Tests;

public class ClassicFoundationTests(ITestContextAccessor testContext, TestWebAppFixture fixture)
    : WebAppFixtureTest<TestWebAppFixture>(testContext, fixture)
{
    [Fact]
    public async Task Starts()
    {
        var response = await AlbaHost.Server.CreateClient().GetAsync("/");
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    //    [Fact]
    //    public async Task OpenApiDocument()
    //    {
    //        var response = await AlbaHost.Server.CreateClient().GetAsync("/openapi/v1.json");
    //        var document = await response.Content.ReadAsStringAsync();
    //        await VerifyJson(document);
    //    }
}
