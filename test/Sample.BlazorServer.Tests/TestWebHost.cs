using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Sample.Core.Domain;

namespace Sample.BlazorServer.Tests;

public class TestWebHost : ConventionTestWebHost<Startup>
{
    private readonly SqliteConnection _connection;

    public TestWebHost()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
    }

    protected override IHostBuilder CreateHostBuilder()
    {
        _connection.Open();
        return base.CreateHostBuilder()
                   .ConfigureServices(
                        (context, services) =>
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
