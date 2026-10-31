using DryIoc;
using DryIoc.Microsoft.DependencyInjection;

using Joinery.Dovetail;
using Joinery.Dovetail.Joints;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Sample.Core.Domain;

using Serilog.Events;

using TUnit.Core.Interfaces;

using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Sample.Core.Tests;

public abstract class HandleTestHostBase : AutoFakeTest<TestRecord>, IAsyncInitializer
{
    public async Task InitializeAsync()
    {
        _connection = new("DataSource=:memory:");
        await _connection.OpenAsync(TestContext.CancellationToken);

        // the imported joints: the generated Imports seed the Specular provider and, in a test project, the unit test host type
        var context = await DovetailContext.FromAsync(Imports.Joints(), TestContext.CancellationToken);
        var configuration = new ConfigurationManager();
        await configuration.ApplyConfiguration(context, TestContext.CancellationToken);
        context.Set<IConfiguration>(configuration);

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddDbContextPool<RocketDbContext>(
            z => z
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .UseSqlite(_connection)
        );
        await services.ApplyService(context, TestContext.CancellationToken);
        Populate(services);
        await using var scope = ServiceProvider.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<RocketDbContext>().Database.EnsureCreatedAsync(TestContext.CancellationToken);
    }

    [After(Test)]
    public async Task CloseConnectionAsync()
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }
    }

    protected HandleTestHostBase() : this(LogEventLevel.Information) { }

    protected HandleTestHostBase(LogEventLevel logLevel) : base(TestRecord.Create(logLevel)) => ExcludeSourceContext(nameof(AutoFakeTest));

    protected override IContainer BuildContainer(IContainer container) => container.WithDependencyInjectionAdapter().Container;
    private SqliteConnection? _connection;
}
