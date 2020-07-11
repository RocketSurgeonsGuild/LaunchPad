using System;
using System.Linq;
using System.Threading.Tasks;
using DryIoc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.TestHost;
using Rocket.Surgery.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.Extensions.Conventions;
using Sample.Core.Domain;
using Serilog;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace Sample.Core.Tests
{
    public abstract class HandleTestHostBase : AutoFakeTest, IAsyncLifetime
    {
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
            _hostBuilder.Scanner.ExceptConvention(typeof(NodaTimeConvention));
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

            Populate(_hostBuilder.Parse());

            await ServiceProvider.WithScoped<RocketDbContext>().Invoke(context => context.Database.EnsureCreatedAsync());
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