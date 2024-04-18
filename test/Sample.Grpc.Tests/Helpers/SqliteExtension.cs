using Alba;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.DependencyInjection;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;

namespace Sample.Grpc.Tests.Helpers;

public sealed class SqliteExtension<TDbContext> : IResettableAlbaExtension where TDbContext : DbContext
{
    private readonly SqliteConnection _connection;

    public SqliteExtension()
    {
        _connection = new("DataSource=:memory:");
    }

    public void Reset(IServiceProvider serviceProvider)
    {
        _connection.Close();
        _connection.Open();
        serviceProvider.WithScoped<TDbContext>().Invoke(c => c.Database.EnsureCreated());
    }

    public async Task ResetAsync(IServiceProvider serviceProvider)
    {
        await _connection.CloseAsync();
        await _connection.OpenAsync();
        await serviceProvider.WithScoped<TDbContext>().Invoke(c => c.Database.EnsureCreatedAsync());
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return _connection.DisposeAsync();
    }

    public Task Start(IAlbaHost host)
    {
        return Task.CompletedTask;
    }

    public IHostBuilder Configure(IHostBuilder builder)
    {
        builder
           .ConfigureServices(
                (_, services) =>
                {
                    services.AddDbContextPool<TDbContext>(
                        z => z
                            .EnableDetailedErrors()
                            .EnableSensitiveDataLogging()
                            .EnableThreadSafetyChecks()
                            .EnableServiceProviderCaching(false)
                            .UseSqlite(_connection)
                    );
                }
            );

        return builder;
    }
}