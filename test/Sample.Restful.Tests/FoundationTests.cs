using System.Net;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Sample.BlazorServer.Tests;
using Sample.Restful.Tests.Helpers;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Sample.Restful.Tests;

public class FoundationTests : WebAppFixtureTest<TestWebAppFixture>
{
    [Fact]
    public void AutoMapper()
    {
        AlbaHost.Services.GetRequiredService<IMapper>()
                .ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public async Task Starts()
    {
        var response = await AlbaHost.Server.CreateClient().GetAsync("/");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public FoundationTests(ITestOutputHelper testOutputHelper, TestWebAppFixture factory) : base(testOutputHelper, factory)
    {
    }

    [Theory]
    [ClassData(typeof(OpenApiDocuments))]
    public void OpenApiDocument(string document)
    {
        AlbaHost.Services.GetRequiredService<ISwaggerProvider>()
                .GetSwagger(document).Should().NotBeNull();
    }

    private class OpenApiDocuments : TheoryData<string>
    {
        public OpenApiDocuments()
        {
            using var host = new TestWebAppFixture();
            host.InitializeAsync().GetAwaiter().GetResult();
            foreach (var item in host.AlbaHost.Services.GetRequiredService<IOptions<SwaggerGeneratorOptions>>().Value.SwaggerDocs.Keys)
            {
                Add(item);
            }
        }
    }
}
