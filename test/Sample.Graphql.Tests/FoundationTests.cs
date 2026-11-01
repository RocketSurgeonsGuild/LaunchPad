using System.Net;

using Alba;

using HotChocolate.Execution;

using Rocket.Surgery.LaunchPad.AspNetCore.Testing;

using Sample.Core.Domain;
using Sample.Graphql.Tests.Helpers;

namespace Sample.Graphql.Tests;

public class FoundationTests() : LoggerTest<TestRecord>(TestRecord.Create())
{
    [Test]
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

    [Test]
    public async Task Starts_Without_The_Test_Services()
    {
        // LaunchPadExtension and GraphQlExtension register services (HTTP logging among them) the sample has to register itself when it
        // runs; the database stands in for the one the sample only registers live
        await using var host = await AlbaHost.For<Program>(new SqliteExtension<RocketDbContext>());
        var response = await host.Server.CreateClient().GetAsync("/graphql/index.html", TestContext.CancellationToken);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Test]
    [Skip("snapshots not consistent atm")]
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
