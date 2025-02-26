using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.Conventions;

using Serilog.Events;

namespace Sample.BlazorWasm.Tests;

public abstract class HandleTestHostBase
    (ITestContextAccessor outputHelper, LogEventLevel logLevel = LogEventLevel.Information) : AutoFakeTest<XUnitTestContext>(new XUnitTestContext(outputHelper, logLevel)), IAsyncLifetime
{
    public async ValueTask InitializeAsync()
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

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    private IConventionContext _hostBuilder = null!;
}
