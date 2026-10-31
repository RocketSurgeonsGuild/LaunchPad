using Joinery.Dovetail;
using Joinery.Dovetail.Joints;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Serilog.Events;

using TUnit.Core.Interfaces;

using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Sample.BlazorWasm.Tests;

public abstract class HandleTestHostBase(LogEventLevel logLevel) : AutoFakeTest<TestRecord>(TestRecord.Create(logLevel)), IAsyncInitializer
{
    protected HandleTestHostBase() : this(LogEventLevel.Information) { }

    public async Task InitializeAsync()
    {
        ExcludeSourceContext(nameof(WebAssemblyHostBuilder));
        ExcludeSourceContext(nameof(WebAssemblyHost));

        // the imported joints: the generated Imports seed the Specular provider and, in a test project, the unit test host type
        var context = await DovetailContext.FromAsync(Imports.Joints(), TestContext.CancellationToken);
        var configuration = new ConfigurationManager();
        await configuration.ApplyConfiguration(context, TestContext.CancellationToken);
        context.Set<IConfiguration>(configuration);

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        await services.ApplyService(context, TestContext.CancellationToken);
        Populate(services);
    }
}
