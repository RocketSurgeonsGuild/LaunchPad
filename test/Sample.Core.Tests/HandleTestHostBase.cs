using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting;
using Sample.Core.Domain;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sample.Core.Tests
{
    public abstract class HandleTestHostBase : AutoFakeTest, IAsyncLifetime
    {
        private readonly TestHostBuilder _hostBuilder;
        private SqliteConnection _connection;

        protected HandleTestHostBase(ITestOutputHelper outputHelper, LogLevel logLevel = LogLevel.Information) : base(
            outputHelper,
            logLevel,
            logFormat: "[{Timestamp:HH:mm:ss} {Level:w4}] {Message} <{SourceContext}>{NewLine}{Exception}"
        )
        {
            _hostBuilder = TestHost.For(this, LoggerFactory)
               .WithLogger(LoggerFactory.CreateLogger(nameof(TestHost)))
               .Create();
            ExcludeSourceContext(nameof(TestHost));
        }

        public async Task InitializeAsync()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _hostBuilder
               .ConfigureServices(
                    (context, services) =>
                    {
                        services.AddDbContextPool<RocketDbContext>(
                            x => x
                               .EnableDetailedErrors()
                               .EnableSensitiveDataLogging()
                               .UseSqlite(_connection)
                        );
                    }
                );

            Populate(_hostBuilder.Parse());

            await ServiceProvider.WithScoped<RocketDbContext>().Invoke(context => context.Database.EnsureCreatedAsync());
        }

        public async Task DisposeAsync() => await _connection.DisposeAsync();
    }
}