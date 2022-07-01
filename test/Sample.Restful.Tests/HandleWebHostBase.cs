using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Sample.Core.Domain;
using Xunit;
using Xunit.Abstractions;

namespace Sample.Restful.Tests;

[ImportConventions]
public abstract partial class HandleWebHostBase : LoggerTest, IAsyncLifetime
{
    protected HandleWebHostBase(
        ITestOutputHelper outputHelper,
        LogLevel logLevel = LogLevel.Trace
    ) : base(
        outputHelper,
        logLevel,
        "[{Timestamp:HH:mm:ss} {Level:w4}] {Message} <{SourceContext}>{NewLine}{Exception}"
    )
    {
        Factory = new TestWebHost()
           .ConfigureLoggerFactory(LoggerFactory);
    }

    protected ConventionTestWebHost<Program> Factory { get; private set; }
    protected IServiceProvider ServiceProvider => Factory.Services;

    public async Task InitializeAsync()
    {
        await Task.Yield();
        await ServiceProvider.GetRequiredService<RocketDbContext>().Database.EnsureDeletedAsync();
    }

    public async Task DisposeAsync()
    {
        await Factory.DisposeAsync();
    }
}
