using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.TestHost;
using Rocket.Surgery.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Sample.Core.Domain;
using Serilog;
using Serilog.Events;
using xunit;
using Xunit;
using Xunit.Abstractions;

namespace Sample.Core.Tests.Rockets
{
    public abstract class HandleTestHostBase : LoggerTest, IAsyncLifetime
    {
        protected IConfiguration _configuration;
        protected IServiceProvider _serviceProvider;
        private readonly ConventionTestHost _hostBuilder;
        private SqliteConnection _connection;

        protected HandleTestHostBase(ITestOutputHelper outputHelper, LogLevel logLevel = LogLevel.Information) : base(
            outputHelper,
            logLevel,
            logFormat: "[{Timestamp:HH:mm:ss} {Level:w4}] {Message} <{SourceContext}>{NewLine}{Exception}",
            configureLogger: ConfigureLogging
        )
        {
            _hostBuilder = ConventionTestHostBuilder.For(this, LoggerFactory)
               .With(Logger)
               .With(DiagnosticSource)
               .Create();
        }

        public async Task InitializeAsync()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _hostBuilder
               .ConfigureServices(
                    context =>
                    {
                        context.Services.AddDbContext<RocketDbContext>(
                            x => x
                               .EnableDetailedErrors()
                               .EnableSensitiveDataLogging()
                               .UseSqlite(_connection),
                            ServiceLifetime.Scoped,
                            ServiceLifetime.Scoped
                        );
                    }
                );
            ( _configuration, _serviceProvider ) = _hostBuilder.Build();

            await _serviceProvider.WithScoped<RocketDbContext>().Invoke(context => context.Database.EnsureCreatedAsync());
        }

        public async Task DisposeAsync() => await _connection.DisposeAsync();

        static void ConfigureLogging(LoggerConfiguration loggerConfiguration)
        {
            loggerConfiguration.Filter.ByExcluding(
                x =>
                {
                    if (!x.Properties.TryGetValue("SourceContext", out var c) || !( c is ScalarValue sv ) || !( sv.Value is string sourceContext ))
                        return false;
                    return sourceContext.Equals(nameof(ConventionTestHostBuilder))
                     || sourceContext.Equals(nameof(ConventionTestHost))
                     || sourceContext.Equals(nameof(DiagnosticSource));
                }
            );
        }
    }
}