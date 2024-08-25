using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Sample.Grpc.Tests.Helpers;

namespace Sample.Grpc.Tests;

public class FoundationTests(ITestOutputHelper testOutputHelper, TestWebAppFixture factory) : WebAppFixtureTest<TestWebAppFixture>(testOutputHelper, factory)
{
    [Fact]
    public async Task Starts()
    {
        var response = await AlbaHost.Server.CreateClient().GetAsync("/");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
