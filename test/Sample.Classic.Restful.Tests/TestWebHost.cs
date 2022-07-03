﻿using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Sample.Core.Domain;

namespace Sample.Classic.Restful.Tests;

public class TestWebHost : ConventionTestWebHost<Startup>, IAsyncLifetime
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

    public void Reset()
    {
        _connection.Close();
        _connection.Open();
    }

    protected override void Dispose(bool disposing)
    {
        _connection.Close();
        _connection.Dispose();
        base.Dispose(disposing);
    }

    public async Task InitializeAsync()
    {
        await Services.GetRequiredService<RocketDbContext>().Database.EnsureCreatedAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await DisposeAsync();
    }
}
