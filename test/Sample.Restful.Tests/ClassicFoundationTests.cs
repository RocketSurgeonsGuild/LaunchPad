using System.Net;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Sample.Classic.Restful;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Sample.Restful.Tests;

public class ClassicFoundationTests : AutoFakeTest, IClassFixture<TestWebHost<Startup>>
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

    [Fact]
    public void OpenApiDocument()
    {
        var docs = _factory.Services
                           .GetRequiredService<IOptions<SwaggerGeneratorOptions>>().Value.SwaggerDocs.Keys;
        foreach (var document in docs)
        {
            _factory.Services.GetRequiredService<ISwaggerProvider>()
                    .GetSwagger(document).Should().NotBeNull();
        }
    }

    public ClassicFoundationTests(ITestOutputHelper testOutputHelper, TestWebHost<Startup> factory) : base(testOutputHelper)
    {
        _factory = factory.ConfigureLogger(SerilogLogger);
    }

    private readonly ConventionTestWebHost<Startup> _factory;
}
