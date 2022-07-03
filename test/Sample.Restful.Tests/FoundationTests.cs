using System.Net;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Sample.Restful.Tests;

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
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public FoundationTests(ITestOutputHelper testOutputHelper, TestWebHost factory) : base(testOutputHelper)
    {
        _factory = factory.ConfigureLoggerFactory(LoggerFactory);
    }

    private readonly ConventionTestWebHost<Program> _factory;

    [Theory]
    [ClassData(typeof(OpenApiDocuments))]
    public void OpenApiDocument(string document)
    {
        _factory.Services.GetRequiredService<ISwaggerProvider>()
                .GetSwagger(document).Should().NotBeNull();
    }

    private class OpenApiDocuments : TheoryData<string>
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
