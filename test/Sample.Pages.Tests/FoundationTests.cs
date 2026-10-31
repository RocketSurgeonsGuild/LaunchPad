using System.Net;

using Sample.Pages.Tests.Helpers;

namespace Sample.Pages.Tests;

public class FoundationTests : WebAppFixtureTest
{
    [Test]
    public async Task Starts()
    {
        var response = await AlbaHost.Server.CreateClient().GetAsync("/", TestContext.CancellationToken);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
