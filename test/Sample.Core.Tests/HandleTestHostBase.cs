using DryIoc;
using DryIoc.Microsoft.DependencyInjection;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.Conventions;

using Sample.Core.Domain;

using Serilog.Events;

using TUnit.Core.Interfaces;

namespace Sample.Core.Tests;

public abstract class HandleTestHostBase : AutoFakeTest<TestRecord>, IAsyncInitializer
{
    public async Task InitializeAsync()
    {
        _connection = new("DataSource=:memory:");
        await _connection.OpenAsync(TestContext.CancellationToken);
        _context = ConventionContextBuilder
                  .Create(Imports.Instance)
                  .UseLogger(Logger);

        var services = await new ServiceCollection()
                            .AddDbContextPool<RocketDbContext>(
                                 z => z
                                     .EnableDetailedErrors()
                                     .EnableSensitiveDataLogging()
                                     .UseSqlite(_connection)
                             )
                            .ApplyConventionsAsync(await ConventionContext.FromAsync(_context, TestContext.CancellationToken));
        Populate(services);
        await using var scope = ServiceProvider.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<RocketDbContext>().Database.EnsureCreatedAsync(TestContext.CancellationToken);
    }

    [After(HookType.Test)]
    public async Task CloseConnectionAsync()
    {
        if (_connection is not null) await _connection.DisposeAsync();
    }

    protected HandleTestHostBase(LogEventLevel logLevel = LogEventLevel.Information) : base(TestRecord.Create(logLevel)) => ExcludeSourceContext(nameof(AutoFakeTest));

    protected override IContainer BuildContainer(IContainer container) => container.WithDependencyInjectionAdapter().Container;
    private ConventionContextBuilder? _context;
    private SqliteConnection? _connection;
}
