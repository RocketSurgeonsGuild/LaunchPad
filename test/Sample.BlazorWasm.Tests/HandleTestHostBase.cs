using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.Conventions;

using Serilog.Events;

namespace Sample.BlazorWasm.Tests;

public abstract class HandleTestHostBase(ITestOutputHelper outputHelper, LogEventLevel logLevel = LogEventLevel.Information) : AutoFakeTest<XUnitTestContext>(XUnitTestContext.Create(outputHelper, logLevel)), IAsyncLifetime
{
    private IConventionContext _hostBuilder = null!;

    public async Task InitializeAsync()
    {
        _hostBuilder =
            await ConventionContext.FromAsync(
                ConventionContextBuilder
                   .Create(Imports.Instance)
                   .UseLogger(Logger)
            );
        ExcludeSourceContext(nameof(WebAssemblyHostBuilder));
        ExcludeSourceContext(nameof(WebAssemblyHost));
        Populate(await new ServiceCollection().ApplyConventionsAsync(_hostBuilder));
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
