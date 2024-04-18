using System.Runtime.Loader;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Testing;
using Rocket.Surgery.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Sample.Core.Domain;

namespace Sample.Core.Tests;

public abstract class HandleTestHostBase : AutoFakeTest, IAsyncLifetime
{
    private readonly ConventionContextBuilder _context;
    private SqliteConnection? _connection;

    protected HandleTestHostBase(ITestOutputHelper outputHelper, LogLevel logLevel = LogLevel.Information) : base(
        outputHelper,
        logLevel,
        "[{Timestamp:HH:mm:ss} {Level:w4}] {Message} <{SourceContext}>{NewLine}{Exception}"
    )
    {
        _context =
            ConventionContextBuilder.Create()
                                    .ForTesting(Imports.Instance, LoggerFactory)
                                    .Set(AssemblyLoadContext.Default)
                                    .WithLogger(LoggerFactory.CreateLogger(nameof(AutoFakeTest)));
        ExcludeSourceContext(nameof(AutoFakeTest));
    }

    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        await _connection.OpenAsync();

        _context
           .ConfigureServices(
                (_, services) =>
                {
                    services.AddDbContextPool<RocketDbContext>(
                        z => z
                            .EnableDetailedErrors()
                            .EnableSensitiveDataLogging().UseSqlite(
                                 _connection
                             )
                    );
                }
            );

        Populate(await new ServiceCollection().ApplyConventionsAsync(await ConventionContext.FromAsync(_context)));

        await ServiceProvider.WithScoped<RocketDbContext>().Invoke(context => context.Database.EnsureCreatedAsync());
    }

    public async Task DisposeAsync()
    {
        await _connection!.DisposeAsync();
    }
}
