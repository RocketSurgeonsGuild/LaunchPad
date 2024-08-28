using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sample.Restful.Tests.Helpers;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Sample.Restful.Tests;

public class FoundationTests(ITestOutputHelper testOutputHelper, TestWebAppFixture factory) : WebAppFixtureTest<TestWebAppFixture>(testOutputHelper, factory)
{
    [Fact]
    public async Task Starts()
    {
        var response = await AlbaHost.Server.CreateClient().GetAsync("/");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [ClassData(typeof(OpenApiDocuments))]
    public void OpenApiDocument(string document)
    {
        AlbaHost
           .Services.GetRequiredService<ISwaggerProvider>()
           .GetSwagger(document)
           .Should()
           .NotBeNull();
    }

    private sealed class OpenApiDocuments : TheoryData<string>
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