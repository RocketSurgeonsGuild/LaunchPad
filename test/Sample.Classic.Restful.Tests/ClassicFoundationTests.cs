using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sample.Classic.Restful.Tests.Helpers;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Sample.Classic.Restful.Tests;

public class ClassicFoundationTests(ITestOutputHelper testOutputHelper, TestWebAppFixture fixture)
    : WebAppFixtureTest<TestWebAppFixture>(testOutputHelper, fixture)
{
    [Fact]
    public async Task Starts()
    {
        var response = await AlbaHost.Server.CreateClient().GetAsync("/");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public void OpenApiDocument()
    {
        var docs = ServiceProvider
                  .GetRequiredService<IOptions<SwaggerGeneratorOptions>>()
                  .Value.SwaggerDocs.Keys;
        foreach (var document in docs)
        {
            ServiceProvider
               .GetRequiredService<ISwaggerProvider>()
               .GetSwagger(document)
               .Should()
               .NotBeNull();
        }
    }
}