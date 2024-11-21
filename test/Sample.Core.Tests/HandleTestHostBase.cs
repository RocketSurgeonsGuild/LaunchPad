using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Testing;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Serilog.Events;

namespace Sample.Core.Tests;

public abstract class HandleTestHostBase : AutoFakeTest<XUnitTestContext>, IAsyncLifetime
{
    private ConventionContextBuilder? _context;
    private SqliteConnection? _connection;

    protected HandleTestHostBase(ITestOutputHelper outputHelper, LogEventLevel logLevel = LogEventLevel.Information) : base(XUnitTestContext.Create(outputHelper, logLevel))
    {
        var factory = CreateLoggerFactory();
        ExcludeSourceContext(nameof(AutoFakeTest));
    }

    public async Task InitializeAsync()
    {
        _connection = new("DataSource=:memory:");
        await _connection.OpenAsync();
        var factory = CreateLoggerFactory();
        _context = ConventionContextBuilder
                  .Create()
                  .ForTesting(Imports.Instance, factory)
                  .WithLogger(factory.CreateLogger(nameof(AutoFakeTest)))
           .ConfigureServices(
                (_, services) =>
                {
                    services.AddDbContextPool<RocketDbContext>(
                        z => z
                            .EnableDetailedErrors()
                            .EnableSensitiveDataLogging()
                            .UseSqlite(
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
