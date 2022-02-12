using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Sample.Core.Domain;
using Xunit;
using Xunit.Abstractions;

namespace Sample.Restful.Tests;

[ImportConventions]
public abstract partial class HandleWebHostBase : LoggerTest, IAsyncLifetime
{
    private SqliteConnection _connection = null!;

    protected HandleWebHostBase(
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
                          .ConfigureHosting((context, z) => z.ConfigureServices((_, s) => s.AddSingleton(context)))
                           // .WithConventionsFrom(GetConventions)
                          .EnableConventionAttributes()
                  )
                 .ConfigureLoggerFactory(LoggerFactory);
    }

    protected ConventionTestWebHost<Startup> Factory { get; private set; }
    protected IServiceProvider ServiceProvider => Factory.Services;

    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        await _connection.OpenAsync();

        Factory = Factory.ConfigureHostBuilder(
            x => x
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
