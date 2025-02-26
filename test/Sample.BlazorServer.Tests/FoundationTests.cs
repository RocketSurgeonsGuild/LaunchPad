﻿using System.Net;
using Sample.BlazorServer.Tests.Helpers;

namespace Sample.BlazorServer.Tests;

public class FoundationTests(ITestContextAccessor testContext, TestWebAppFixture factory) : WebAppFixtureTest<TestWebAppFixture>(testContext, factory)
{
    [Fact]
    public async Task Starts()
    {
        var response = await AlbaHost.Server.CreateClient().GetAsync("/", TestContext.CancellationToken);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
