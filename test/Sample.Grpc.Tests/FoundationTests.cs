using System.Net;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sample.Grpc.Tests
{
    public class FoundationTests : AutoFakeTest, IClassFixture<TestWebHost>
    {
        private readonly ConventionTestWebHost<Startup> _factory;

        public FoundationTests(ITestOutputHelper testOutputHelper, TestWebHost factory) : base(testOutputHelper) => _factory = factory.ConfigureLoggerFactory(LoggerFactory);

        [Fact]
        public void AutoMapper() => _factory.Services.GetRequiredService<IMapper>()
           .ConfigurationProvider.AssertConfigurationIsValid();

        [Fact]
        public async Task Starts()
        {
            var response = await _factory.CreateClient().GetAsync("/");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
