using System.Net;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Sample.Classic.Restful.Tests.Helpers;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Sample.Classic.Restful.Tests;

public class ClassicFoundationTests : WebAppFixtureTest<TestWebAppFixture>
{
    public ClassicFoundationTests(ITestOutputHelper testOutputHelper, TestWebAppFixture fixture) : base(testOutputHelper, fixture)
    {
    }

    [Fact]
    public void AutoMapper()
    {
        ServiceProvider.GetRequiredService<IMapper>()
                .ConfigurationProvider.AssertConfigurationIsValid();
    }

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
                           .GetRequiredService<IOptions<SwaggerGeneratorOptions>>().Value.SwaggerDocs.Keys;
        foreach (var document in docs)
        {
            ServiceProvider.GetRequiredService<ISwaggerProvider>()
                    .GetSwagger(document).Should().NotBeNull();
        }
    }
}
