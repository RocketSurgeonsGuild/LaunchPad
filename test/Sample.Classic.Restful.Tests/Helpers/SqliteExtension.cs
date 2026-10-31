using Alba;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;

namespace Sample.Classic.Restful.Tests.Helpers;

public sealed class SqliteExtension<TDbContext> : IResettableAlbaExtension where TDbContext : DbContext
{
    private readonly SqliteConnection _connection;

    public SqliteExtension() => _connection = new("DataSource=:memory:");

    public void Reset(IServiceProvider serviceProvider)
    {
        _connection.Close();
        _connection.Open();
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        dbContext.Database.EnsureCreated();
    }

    public async ValueTask ResetAsync(IServiceProvider serviceProvider)
    {
        await _connection.CloseAsync();
        await _connection.OpenAsync();
        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public void Dispose() => _connection.Dispose();

    public ValueTask DisposeAsync() => _connection.DisposeAsync();

    public Task Start(IAlbaHost host) => Task.CompletedTask;

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
