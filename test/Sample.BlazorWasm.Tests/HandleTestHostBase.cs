using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Testing;
using Rocket.Surgery.Extensions.Testing;

namespace Sample.BlazorWasm.Tests;

public abstract class HandleTestHostBase(ITestOutputHelper outputHelper, LogLevel logLevel = LogLevel.Information) : AutoFakeTest(
    outputHelper,
    logLevel,
    "[{Timestamp:HH:mm:ss} {Level:w4}] {Message} <{SourceContext}>{NewLine}{Exception}"
), IAsyncLifetime
{
    private IConventionContext _hostBuilder = null!;

    public async Task InitializeAsync()
    {
        _hostBuilder =
            await ConventionContext.FromAsync(
                ConventionContextBuilder
                   .Create()
                   .ForTesting(Imports.Instance, LoggerFactory)
                   .WithLogger(Logger)
            );
        ExcludeSourceContext(nameof(WebAssemblyHostBuilder));
        ExcludeSourceContext(nameof(WebAssemblyHost));
        Populate(await new ServiceCollection().ApplyConventionsAsync(_hostBuilder));
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
