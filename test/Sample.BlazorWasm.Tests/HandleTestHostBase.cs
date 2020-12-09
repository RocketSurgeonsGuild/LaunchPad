using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.Foundation.Conventions;
using Rocket.Surgery.WebAssembly.Hosting;
using Xunit;
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
               .Create(b => b.ExceptConvention(typeof(NodaTimeConvention)));
            ExcludeSourceContext(nameof(WebAssemblyHostBuilder));
            ExcludeSourceContext(nameof(WebAssemblyHost));
            ExcludeSourceContext(nameof(DiagnosticSource));
        }
    }
}