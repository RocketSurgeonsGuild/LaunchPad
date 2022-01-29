using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Sample.Core.Domain;
using Xunit;
using Xunit.Abstractions;

namespace Sample.Grpc.Tests;

[ImportConventions]
public abstract partial class HandleGrpcHostBase : LoggerTest, IAsyncLifetime
{
    private SqliteConnection _connection = null!;

    protected HandleGrpcHostBase(
        ITestOutputHelper outputHelper,
        LogLevel logLevel = LogLevel.Trace
    ) : base(
        outputHelper,
        logLevel,
        "[{Timestamp:HH:mm:ss} {Level:w4}] {Message} <{SourceContext}>{NewLine}{Exception}"
    )
    {
        Factory = new TestWebHost()
                 .ConfigureHostBuilder(
                      b => b
                          .ConfigureHosting((context, z) => z.ConfigureServices((c, s) => s.AddSingleton(context)))
                          .EnableConventionAttributes()
                  )
                 .ConfigureLoggerFactory(LoggerFactory);
        ExcludeSourceContext("ConventionTestWebHost");
    }

    protected ConventionTestWebHost<Startup> Factory { get; private set; }
    protected IServiceProvider ServiceProvider => Factory.Services;

    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        Factory = Factory.ConfigureHostBuilder(
            x => x
               .ConfigureServices(
                    (_, services) =>
                    {
                        services.AddDbContextPool<RocketDbContext>(
                            x => x
                                .EnableDetailedErrors()
                                .EnableSensitiveDataLogging()
                                .UseSqlite(_connection)
                        );
                    }
                )
        );

        await ServiceProvider.WithScoped<RocketDbContext>().Invoke(context => context.Database.EnsureCreatedAsync());
    }

    public async Task DisposeAsync()
    {
        Factory.Dispose();
        await _connection.DisposeAsync();
    }
}
