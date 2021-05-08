using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.WebAssembly.Hosting;
using System;
using Xunit.Abstractions;

namespace Sample.BlazorWasm.Tests
{
    public abstract class HandleTestHostBase : AutoFakeTest
    {
        private readonly TestWebAssemblyHostBuilder _hostBuilder;

        protected HandleTestHostBase(ITestOutputHelper outputHelper, LogLevel logLevel = LogLevel.Information) : base(
            outputHelper,
            logLevel,
            logFormat: "[{Timestamp:HH:mm:ss} {Level:w4}] {Message} <{SourceContext}>{NewLine}{Exception}"
        )
        {
            _hostBuilder = TestWebAssemblyHost.For(AppDomain.CurrentDomain, LoggerFactory)
               .WithLogger(Logger)
               .Create();
            ExcludeSourceContext(nameof(WebAssemblyHostBuilder));
            ExcludeSourceContext(nameof(WebAssemblyHost));
            ExcludeSourceContext(nameof(DiagnosticSource));
        }
    }
}