using System.Net;
using Alba;
using AutoMapper;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Sample.Core.Domain;
using Sample.Graphql.Tests.Helpers;
using IRequestExecutorResolver = HotChocolate.Execution.IRequestExecutorResolver;

namespace Sample.Graphql.Tests;

[UsesVerify]
public class FoundationTests : LoggerTest
{
    [Fact]
    public async Task AutoMapper()
    {
        await using var host = await AlbaHost.For<Program>(new LaunchPadExtension<FoundationTests>(LoggerFactory), new GraphQlExtension(), new SqliteExtension<RocketDbContext>());
        host.Services.GetRequiredService<IMapper>()
            .ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public async Task Starts()
    {
        await using var host = await AlbaHost.For<Program>(new LaunchPadExtension<FoundationTests>(LoggerFactory), new GraphQlExtension(), new SqliteExtension<RocketDbContext>());
        var response = await host.Server.CreateClient().GetAsync("/graphql/index.html");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GraphqlSchema()
    {
        await using var host = await AlbaHost.For<Program>(new LaunchPadExtension<FoundationTests>(LoggerFactory), new GraphQlExtension(), new SqliteExtension<RocketDbContext>());
        var exeuctor = await host.Services.GetRequestExecutorAsync();
        await Verify(exeuctor.Schema.Print(), "graphql");
    }

    public FoundationTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }
}
