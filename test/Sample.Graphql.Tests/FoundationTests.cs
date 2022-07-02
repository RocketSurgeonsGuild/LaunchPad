using System.Net;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using IRequestExecutorResolver = HotChocolate.Execution.IRequestExecutorResolver;

namespace Sample.Graphql.Tests;

public class FoundationTests : AutoFakeTest, IClassFixture<TestWebHost>
{
    [Fact]
    public void AutoMapper()
    {
        _factory.Services.GetRequiredService<IMapper>()
                .ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public async Task Starts()
    {
        var response = await _factory.CreateClient().GetAsync("/graphql/index.html");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GraphqlSchema()
    {
        var exeuctor = await _factory.Services.WithScoped<IRequestExecutorResolver>().Invoke(z => z.GetRequestExecutorAsync());
        Logger.LogInformation(exeuctor.Schema.Print());
    }

    public FoundationTests(ITestOutputHelper testOutputHelper, TestWebHost factory) : base(testOutputHelper)
    {
        _factory = factory.ConfigureLoggerFactory(LoggerFactory);
    }

    private readonly ConventionTestWebHost<Startup> _factory;
}
