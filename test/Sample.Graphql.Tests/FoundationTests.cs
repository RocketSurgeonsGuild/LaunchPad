using System.Net;
using Alba;
using HotChocolate.Execution;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Sample.Core.Domain;
using Sample.Graphql.Tests.Helpers;

namespace Sample.Graphql.Tests;

public class FoundationTests(ITestContextAccessor testContext) : LoggerTest<XUnitTestContext>(XUnitDefaults.CreateTestContext(testContext))
{
    [Fact]
    public async Task Starts()
    {
        await using var host = await AlbaHost.For<Program>(
            new LaunchPadExtension<FoundationTests>(CreateLoggerFactory()),
            new GraphQlExtension(),
            new SqliteExtension<RocketDbContext>()
        );
        var response = await host.Server.CreateClient().GetAsync("/graphql/index.html", TestContext.CancellationToken);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact(Skip = "snapshots not consistent atm")]
    public async Task GraphqlSchema()
    {
        await using var host = await AlbaHost.For<Program>(
            new LaunchPadExtension<FoundationTests>(CreateLoggerFactory()),
            new GraphQlExtension(),
            new SqliteExtension<RocketDbContext>()
        );
        var exeuctor = await host.Services.GetRequestExecutorAsync(cancellationToken: TestContext.CancellationToken);
        await Verify(exeuctor.Schema.Print(), "txt");
    }
}
