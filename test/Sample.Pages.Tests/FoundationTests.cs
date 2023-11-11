using System.Net;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Sample.Pages.Tests.Helpers;

namespace Sample.Pages.Tests;

public class FoundationTests(ITestOutputHelper testOutputHelper, TestWebAppFixture factory) : WebAppFixtureTest<TestWebAppFixture>(testOutputHelper, factory)
{
    [Fact]
    public void AutoMapper()
    {
        AlbaHost.Services.GetRequiredService<IMapper>()
                .ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public async Task Starts()
    {
        var response = await AlbaHost.Server.CreateClient().GetAsync("/");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
