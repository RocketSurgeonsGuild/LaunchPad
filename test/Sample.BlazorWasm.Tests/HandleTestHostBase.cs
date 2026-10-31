using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.Conventions;

using Serilog.Events;

using TUnit.Core.Interfaces;

namespace Sample.BlazorWasm.Tests;

public abstract class HandleTestHostBase(LogEventLevel logLevel = LogEventLevel.Information) : AutoFakeTest<TestRecord>(TestRecord.Create(logLevel)), IAsyncInitializer
{
    public async Task InitializeAsync()
    {
        _hostBuilder =
            await ConventionContext.FromAsync(
                ConventionContextBuilder
                   .Create(Imports.Instance)
                   .UseLogger(Logger),
                TestContext.CancellationToken
            );
        ExcludeSourceContext(nameof(WebAssemblyHostBuilder));
        ExcludeSourceContext(nameof(WebAssemblyHost));
        Populate(await new ServiceCollection().ApplyConventionsAsync(_hostBuilder));
    }

    private IConventionContext _hostBuilder = null!;
}
