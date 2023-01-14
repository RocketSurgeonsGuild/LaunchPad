using System.Net;
using Alba;
using HotChocolate.AspNetCore.Serialization;
using HotChocolate.Execution;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Testing;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Sample.Core.Domain;

namespace Sample.Graphql.Tests;

public class TestWebHost : ConventionTestWebHost<Program>
{
    private SqliteConnection _connection;

    public TestWebHost()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
    }

    protected override IHostBuilder CreateHostBuilder()
    {
        _connection.Open();
#pragma warning disable CS8602
        return base.CreateHostBuilder()
#pragma warning restore CS8602
                   .ConfigureServices(
                        (_, services) =>
                        {
                            services.AddHostedService<SqliteConnectionService>();
                            services.AddDbContextPool<RocketDbContext>(
                                x => x
                                    .EnableDetailedErrors()
                                    .EnableSensitiveDataLogging()
                                    .UseSqlite(_connection)
                            );
                        }
                    );
    }

    protected override void Dispose(bool disposing)
    {
        _connection.Dispose();
        base.Dispose(disposing);
    }
}

internal class RocketSurgeryExtension : IAlbaExtension
{
    private readonly ILoggerFactory _loggerFactory;

    public RocketSurgeryExtension(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

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
        new TemporaryExtension().Configure(builder);
        builder.ConfigureRocketSurgery(
            z => { z.ForTesting(DependencyContext.Load(typeof(RocketSurgeryExtension).Assembly), _loggerFactory).Set(HostType.UnitTest); }
        );
        builder.ConfigureLogging((context, loggingBuilder) => loggingBuilder.Services.AddSingleton(_loggerFactory));
        builder.ConfigureServices(s => s.AddSingleton<TestServer>(z => (TestServer)z.GetRequiredService<IServer>()));
        builder.ConfigureServices(
            s => s.AddHttpLogging(
                options => { options.LoggingFields = HttpLoggingFields.All; }
            )
        );

        return builder;
    }
}

[Obsolete("Remove once https://github.com/ChilliCream/hotchocolate/issues/5684 is fixed!")]
internal class TemporaryExtension : IAlbaExtension
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
        builder.ConfigureServices(z => z.AddHttpResponseFormatter<MyHttpResponseFormatter>());
        return builder;
    }
}

[Obsolete("Remove once https://github.com/ChilliCream/hotchocolate/issues/5684 is fixed!")]
internal class MyHttpResponseFormatter : DefaultHttpResponseFormatter
{
    public MyHttpResponseFormatter() : base(true)
    {
    }

    protected override HttpStatusCode GetStatusCode(
        IResponseStream responseStream, FormatInfo format,
        HttpStatusCode? proposedStatusCode
    )
    {
        var code = base.GetStatusCode(responseStream, format, proposedStatusCode);
        return code == HttpStatusCode.InternalServerError ? HttpStatusCode.OK : code;
    }

    protected override HttpStatusCode GetStatusCode(
        IQueryResult result, FormatInfo format,
        HttpStatusCode? proposedStatusCode
    )
    {
        var code = base.GetStatusCode(result, format, proposedStatusCode);
        return code == HttpStatusCode.InternalServerError ? HttpStatusCode.OK : code;
    }
}

internal class LocalExtension : IAlbaExtension
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

internal class SqliteExtension : IAlbaExtension
{
    private readonly SqliteConnection _connection;

    public SqliteExtension()
    {
        _connection = new SqliteConnection($"Data Source={Guid.NewGuid():N};Mode=Memory;Cache=Shared");
    }

    public void Reset()
    {
        _connection.Close();
    }

    public async Task ResetAsync()
    {
        _connection.ConnectionString = $"Data Source={Guid.NewGuid():N};Mode=Memory;Cache=Shared";
        await _connection.CloseAsync();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return _connection.DisposeAsync();
    }

    public Task Start(IAlbaHost host)
    {
        return Task.CompletedTask;
    }

    public IHostBuilder Configure(IHostBuilder builder)
    {
        builder.ConfigureServices(
                    s =>
                    {
                        s.AddHttpClient();
                        s.AddRocketClient();
                        s.ConfigureOptions<CO>();
                    }
                )
               .ConfigureServices(
                    (_, services) =>
                    {
                        services.AddDbContextPool<RocketDbContext>(
                            z => z
                                .EnableDetailedErrors()
                                .EnableSensitiveDataLogging()
                                .UseSqlite(_connection)
                        );
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
