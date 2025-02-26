using System.Net;

using Sample.Restful.Tests.Helpers;

namespace Sample.Restful.Tests;

public class FoundationTests(ITestContextAccessor testContext, TestWebAppFixture factory) : WebAppFixtureTest<TestWebAppFixture>(testContext, factory)
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
