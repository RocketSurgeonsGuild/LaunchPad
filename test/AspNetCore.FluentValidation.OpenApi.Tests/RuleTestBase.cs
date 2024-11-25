using System.Runtime.CompilerServices;
using DiffEngine;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi;

namespace AspNetCore.FluentValidation.OpenApi.Tests;

[Experimental(Constants.ExperimentalId)]
public class RequiredPropertyRuleTests : RuleTestBase<RequiredPropertyRule>
{
    [Fact]
    public async Task Should_generate_required_property()
    {
        var response = await _client.GetAsync("/openapi/v1.json");
        response.EnsureSuccessStatusCode();
        await VerifyJson(response.Content.ReadAsStreamAsync());
    }
}

static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        DiffTools.UseOrder(DiffTool.Rider, DiffTool.VisualStudioCode, DiffTool.VisualStudio);
//        Verify
    }
}

#pragma warning disable RSGEXP
public class RuleTestBase<TRule> : IAsyncLifetime where TRule : IPropertyRuleHandler
#pragma warning restore RSGEXP
{
    protected WebApplication _app { get; private set; }
    protected HttpClient _client { get; private set; }

    public async Task InitializeAsync()
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddOpenApi();
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddFluentValidationOpenApi();

        var app = _app = builder.Build();

        app.MapOpenApi();

        app.MapGet("/", () => "Hello world!");

        await app.StartAsync().ConfigureAwait(false);
        var testServer = (TestServer)app.Services.GetRequiredService<IServer>();
        _client = testServer.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await _app.StopAsync().ConfigureAwait(false);
        await _app.DisposeAsync().ConfigureAwait(false);
    }
}
