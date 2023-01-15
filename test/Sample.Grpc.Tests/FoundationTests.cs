using System.Net;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Sample.Grpc.Tests.Helpers;

namespace Sample.Grpc.Tests;

public class FoundationTests : WebAppFixtureTest<TestWebAppFixture>
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

    public FoundationTests(ITestOutputHelper testOutputHelper, TestWebAppFixture factory) : base(testOutputHelper, factory)
    {
    }
}
