using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Sample.Grpc.Tests;

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
        var response = await _factory.CreateClient().GetAsync("/");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    public FoundationTests(ITestOutputHelper testOutputHelper, TestWebHost factory) : base(testOutputHelper)
    {
        _factory = factory.ConfigureLoggerFactory(LoggerFactory);
    }

    private readonly ConventionTestWebHost<Startup> _factory;
}
