﻿using System.Net;
using Alba;
using HotChocolate.Execution;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Sample.Core.Domain;
using Sample.Graphql.Tests.Helpers;

namespace Sample.Graphql.Tests;

public class FoundationTests(ITestOutputHelper testOutputHelper) : LoggerTest<XUnitTestContext>(XUnitDefaults.CreateTestContext(testOutputHelper))
{
    [Fact]
    public async Task Starts()
    {
        await using var host = await AlbaHost.For<Program>(
            new LaunchPadExtension<FoundationTests>(CreateLoggerFactory()),
            new GraphQlExtension(),
            new SqliteExtension<RocketDbContext>()
        );
        var response = await host.Server.CreateClient().GetAsync("/graphql/index.html");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GraphqlSchema()
    {
        await using var host = await AlbaHost.For<Program>(
            new LaunchPadExtension<FoundationTests>(CreateLoggerFactory()),
            new GraphQlExtension(),
            new SqliteExtension<RocketDbContext>()
        );
        var exeuctor = await host.Services.GetRequestExecutorAsync();
        await Verify(exeuctor.Schema.Print(), "graphql");
    }
}
