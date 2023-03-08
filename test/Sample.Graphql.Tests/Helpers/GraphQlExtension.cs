using Alba;
using HotChocolate;
using HotChocolate.AspNetCore.Instrumentation;
using HotChocolate.Execution.Instrumentation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Sample.Graphql.Tests.Helpers;

internal class GraphQlExtension : IAlbaExtension
{
    public void Dispose()
    {
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public Task Start(IAlbaHost host)
    {
        return Task.CompletedTask;
    }

    public IHostBuilder Configure(IHostBuilder builder)
    {
        new TemporaryGraphQlExtension().Configure(builder);
        builder.ConfigureServices(
            z => z
                .AddW3CLogging(z => { })
                .AddHttpLogging(z => { })
                .AddGraphQLServer()
                .AddDiagnosticEventListener<TestServerDiagnosticEventListener>()
                .ModifyRequestOptions(
                     opt => { opt.IncludeExceptionDetails = true; }
                 )
        );
        builder.ConfigureServices(
            s =>
            {
                s.AddHttpClient();
                s.AddRocketClient();
                s.ConfigureOptions<CO>();
            }
        );

        return builder;
    }

    public class CO : PostConfigureOptions<HttpClientFactoryOptions>
    {
        private readonly TestServer _testServer;

        public CO(TestServer testServer) : base(Options.DefaultName, null)
        {
            _testServer = testServer;
        }

        public override void PostConfigure(string name, HttpClientFactoryOptions options)
        {
            options.HttpMessageHandlerBuilderActions.Add(
                builder => builder.PrimaryHandler = _testServer.CreateHandler()
            );

            options.HttpClientActions.Add(
                client => client.BaseAddress = new Uri(_testServer.BaseAddress + "graphql/")
            );
        }
    }
}

public class TestServerDiagnosticEventListener : ServerDiagnosticEventListener
{
    private readonly ILogger<TestServerDiagnosticEventListener> _logger;

    public TestServerDiagnosticEventListener(ILogger<TestServerDiagnosticEventListener> logger)
    {
        _logger = logger;
    }

    public override void HttpRequestError(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "HttpRequestError");
    }

    public override void HttpRequestError(HttpContext context, IError error)
    {
        _logger.LogError(error.Exception, "HttpRequestError");
    }
}
