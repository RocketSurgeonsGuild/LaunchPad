using Alba;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
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
