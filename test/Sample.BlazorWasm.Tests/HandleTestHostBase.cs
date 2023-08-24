using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Testing;
using Rocket.Surgery.Extensions.Testing;

namespace Sample.BlazorWasm.Tests;

public abstract class HandleTestHostBase : AutoFakeTest
{
    private readonly IConventionContext _hostBuilder;

    protected HandleTestHostBase(ITestOutputHelper outputHelper, LogLevel logLevel = LogLevel.Information) : base(
        outputHelper,
        logLevel,
        "[{Timestamp:HH:mm:ss} {Level:w4}] {Message} <{SourceContext}>{NewLine}{Exception}"
    )
    {
        _hostBuilder =
            ConventionContext.From(
                ConventionContextBuilder.Create()
                                        .ForTesting(AppDomain.CurrentDomain, LoggerFactory)
                                        .WithLogger(Logger)
            );
        ExcludeSourceContext(nameof(WebAssemblyHostBuilder));
        ExcludeSourceContext(nameof(WebAssemblyHost));
        Populate(new ServiceCollection().ApplyConventions(_hostBuilder));
    }
}
