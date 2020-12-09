using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Sample.Core.Domain;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sample.Restful.Tests
{
    public abstract class HandleWebHostBase : LoggerTest, IAsyncLifetime
    {
        private SqliteConnection _connection;

        protected ConventionTestWebHost<Startup> Factory { get; private set; }
        protected IServiceProvider ServiceProvider => Factory.Services;

        protected HandleWebHostBase(ITestOutputHelper outputHelper, LogLevel logLevel = LogLevel.Trace) : base(
            outputHelper,
            logLevel,
            logFormat: "[{Timestamp:HH:mm:ss} {Level:w4}] {Message} <{SourceContext}>{NewLine}{Exception}"
        )
        {
            Factory = new TestWebHost().ConfigureLogger(SerilogLogger);
        }

        public async Task InitializeAsync()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            Factory = Factory.ConfigureHostBuilder(
                x => x
                   .ConfigureServices(
                        (_, services) =>
                        {
                            services.AddDbContext<RocketDbContext>(
                                x => x
                                   .EnableDetailedErrors()
                                   .EnableSensitiveDataLogging()
                                   .UseSqlite(_connection),
                                ServiceLifetime.Scoped,
                                ServiceLifetime.Scoped
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
}