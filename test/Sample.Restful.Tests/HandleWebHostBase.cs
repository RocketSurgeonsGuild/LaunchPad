using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Extensions.Testing;
using Sample.Core.Domain;

namespace Sample.Restful.Tests;

public abstract partial class HandleWebHostBase<TProgramOrStartup> : LoggerTest, IAsyncLifetime, IClassFixture<TestWebHost<TProgramOrStartup>>
    where TProgramOrStartup : class
{
    protected HandleWebHostBase(
        ITestOutputHelper outputHelper,
        TestWebHost<TProgramOrStartup> host,
        LogLevel logLevel = LogLevel.Trace
    ) : base(outputHelper, logLevel)
    {
        Factory = host;
    }

    protected TestWebHost<TProgramOrStartup> Factory { get; private set; }
    protected IServiceProvider ServiceProvider => Factory.Services;

    public async Task InitializeAsync()
    {
        await ServiceProvider.GetRequiredService<RocketDbContext>().Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync()
    {
        Factory.Reset();
        return Task.CompletedTask;
    }
}
