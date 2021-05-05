using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sample.Restful.Tests
{
    public class FoundationTests : AutoFakeTest, IClassFixture<TestWebHost>
    {
        private readonly ConventionTestWebHost<Startup> _factory;

        public FoundationTests(ITestOutputHelper testOutputHelper, TestWebHost factory) : base(testOutputHelper)
            => _factory = factory.ConfigureLoggerFactory(LoggerFactory);

        [Fact]
        public void AutoMapper() => _factory.Services.GetRequiredService<IMapper>()
           .ConfigurationProvider.AssertConfigurationIsValid();

        [Fact]
        public async Task Starts()
        {
            var response = await _factory.CreateClient().GetAsync("/");
            response.StatusCode.Should().Be(404);
        }

        [Theory]
        [ClassData(typeof(OpenApiDocuments))]
        public void OpenApiDocument(string document) => _factory.Services.GetRequiredService<ISwaggerProvider>()
           .GetSwagger(document).Should().NotBeNull();

        class OpenApiDocuments : TheoryData<string>
        {
            public OpenApiDocuments()
            {
                using var host = new TestWebHost();
                foreach (var item in host.Services.GetRequiredService<IOptions<SwaggerGeneratorOptions>>().Value.SwaggerDocs.Keys)
                {
                    Add(item);
                }
            }
        }
    }
}