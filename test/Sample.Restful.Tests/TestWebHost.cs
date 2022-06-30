using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Sample.Core.Domain;

namespace Sample.Restful.Tests;

public class TestWebHost : ConventionTestWebHost<Program>
{
    private SqliteConnection _connection;

    public TestWebHost()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }


    protected override IHost CreateHost(IHostBuilder builder)
    {
        return base.CreateHost(
            builder
               .ConfigureServices(
                    (_, services) =>
                    {
                        services.AddHostedService<SqliteConnectionService>();
                        services.AddDbContextPool<RocketDbContext>(
                            x => x
                                .EnableDetailedErrors()
                                .EnableSensitiveDataLogging()
                                .UseSqlite(_connection)
                        );
                    }
                )
        );
    }

    protected override void Dispose(bool disposing)
    {
        _connection.Close();
        _connection.Dispose();
        base.Dispose(disposing);
    }
}
