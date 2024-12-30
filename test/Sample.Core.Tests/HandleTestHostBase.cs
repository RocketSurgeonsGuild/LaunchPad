using DryIoc;
using DryIoc.Microsoft.DependencyInjection;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.DependencyInjection;

using Sample.Core.Domain;

using Serilog.Events;

namespace Sample.Core.Tests;

public abstract class HandleTestHostBase : AutoFakeTest<XUnitTestContext>, IAsyncLifetime
{
    private ConventionContextBuilder? _context;
    private SqliteConnection? _connection;

    protected HandleTestHostBase(ITestOutputHelper outputHelper, LogEventLevel logLevel = LogEventLevel.Information) : base(
        XUnitTestContext.Create(outputHelper, logLevel)
    )
    {
        ExcludeSourceContext(nameof(AutoFakeTest));
    }

    public async Task InitializeAsync()
    {
        _connection = new("DataSource=:memory:");
        await _connection.OpenAsync();
        _context = ConventionContextBuilder.Create(Imports.Instance);

        var services = await new ServiceCollection()
                            .AddDbContextPool<RocketDbContext>(
                                 z => z
                                     .EnableDetailedErrors()
                                     .EnableSensitiveDataLogging()
                                     .UseSqlite(_connection)
                             )
           .ApplyConventionsAsync(await ConventionContext.FromAsync(_context));
        Populate(services);
        await Container.WithScoped<RocketDbContext>().Invoke(context => context.Database.EnsureCreatedAsync());
    }

    public async Task DisposeAsync() => await _connection!.DisposeAsync();

    protected override IContainer BuildContainer(IContainer container) => container.WithDependencyInjectionAdapter().Container;
}
