using System.Net;

using Sample.Restful.Tests.Helpers;

namespace Sample.Restful.Tests;

public class FoundationTests : WebAppFixtureTest
{
    [Test]
    public async Task Starts()
    {
        var response = await AlbaHost.Server.CreateClient().GetAsync("/", TestContext.CancellationToken);
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
