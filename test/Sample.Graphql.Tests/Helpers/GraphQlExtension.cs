using System.Diagnostics;
using Alba;
using HotChocolate;
using HotChocolate.AspNetCore.Instrumentation;
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
    public void Dispose() { }

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
        builder.ConfigureServices(
            z => z
                .AddW3CLogging(_ => { })
                .AddHttpLogging(_ => { })
                .AddGraphQL()
                .AddDiagnosticEventListener<TestServerDiagnosticEventListener>()
                .ModifyRequestOptions(
                     opt => opt.IncludeExceptionDetails = true
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

    public class CO(TestServer testServer) : PostConfigureOptions<HttpClientFactoryOptions>(Options.DefaultName, null)
    {
        public override void PostConfigure(string? name, HttpClientFactoryOptions options)
        {
            options.HttpMessageHandlerBuilderActions.Add(
                builder => builder.PrimaryHandler = testServer.CreateHandler()
            );

            options.HttpClientActions.Add(
                client => client.BaseAddress = new(testServer.BaseAddress + "graphql/")
            );
        }
    }
}

public class TestServerDiagnosticEventListener(ILogger<TestServerDiagnosticEventListener> logger) : ServerDiagnosticEventListener
{
    public override void HttpRequestError(HttpContext context, Exception exception)
    {
        logger.LogError(exception, "HttpRequestError");
    }

    public override void HttpRequestError(HttpContext context, IError error)
    {
        logger.LogError(error.Exception, "HttpRequestError");
    }
}
