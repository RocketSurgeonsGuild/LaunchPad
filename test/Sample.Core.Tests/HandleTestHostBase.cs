using System.Threading.Tasks;
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
using Xunit;
using Xunit.Abstractions;

namespace Sample.Core.Tests;

public abstract class HandleTestHostBase : AutoFakeTest, IAsyncLifetime
{
    private readonly ConventionContextBuilder _context;
    private SqliteConnection _connection;

    protected HandleTestHostBase(ITestOutputHelper outputHelper, LogLevel logLevel = LogLevel.Information) : base(
        outputHelper,
        logLevel,
        "[{Timestamp:HH:mm:ss} {Level:w4}] {Message} <{SourceContext}>{NewLine}{Exception}"
    )
    {
        _context =
            ConventionContextBuilder.Create()
                                    .ForTesting(DependencyContext.Load(GetType().Assembly), LoggerFactory)
                                    .WithLogger(LoggerFactory.CreateLogger(nameof(AutoFakeTest)));
        ExcludeSourceContext(nameof(AutoFakeTest));
    }

    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _context
           .ConfigureServices(
                (context, services) =>
                {
                    services.AddDbContextPool<RocketDbContext>(
                        x => SqliteDbContextOptionsBuilderExtensions.UseSqlite(
                            x
                               .EnableDetailedErrors()
                               .EnableSensitiveDataLogging(), _connection
                        )
                    );
                }
            );

        Populate(new ServiceCollection().ApplyConventions(ConventionContext.From(_context)));

        await ServiceProvider.WithScoped<RocketDbContext>().Invoke(context => context.Database.EnsureCreatedAsync());
    }

    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
    }
}
