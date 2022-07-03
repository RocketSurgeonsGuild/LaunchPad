using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Extensions.Testing;
using Sample.Core.Domain;

namespace Sample.Classic.Restful.Tests;

public abstract partial class HandleWebHostBase : LoggerTest, IAsyncLifetime, IClassFixture<TestWebHost>
{
    protected HandleWebHostBase(
        ITestOutputHelper outputHelper,
        TestWebHost host,
        LogLevel logLevel = LogLevel.Trace
    ) : base(outputHelper, logLevel)
    {
        Factory = host;
    }

    protected TestWebHost Factory { get; private set; }
    protected IServiceProvider ServiceProvider => Factory.Services;

    public async Task InitializeAsync()
    {
        await ServiceProvider.GetRequiredService<RocketDbContext>().Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await ServiceProvider.GetRequiredService<RocketDbContext>().Database.EnsureDeletedAsync();
        Factory.Reset();
    }
}
