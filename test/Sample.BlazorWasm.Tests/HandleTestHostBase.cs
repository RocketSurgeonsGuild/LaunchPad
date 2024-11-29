using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Testing;
using Serilog.Events;

namespace Sample.BlazorWasm.Tests;

public abstract class HandleTestHostBase(ITestOutputHelper outputHelper, LogEventLevel logLevel = LogEventLevel.Information) : AutoFakeTest<XUnitTestContext>(XUnitTestContext.Create(outputHelper, logLevel)), IAsyncLifetime
{
    private IConventionContext _hostBuilder = null!;

    public async Task InitializeAsync()
    {
        var loggerFactory = CreateLoggerFactory();
        _hostBuilder =
            await ConventionContext.FromAsync(
                ConventionContextBuilder
                   .Create()
                   .ForTesting(Imports.Instance, loggerFactory)
                   .WithLogger(loggerFactory.CreateLogger("Test"))
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
