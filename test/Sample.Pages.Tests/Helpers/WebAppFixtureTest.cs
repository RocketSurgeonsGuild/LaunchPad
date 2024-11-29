using Alba;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Serilog;
using Serilog.Events;

namespace Sample.Pages.Tests.Helpers;

public abstract class WebAppFixtureTest<TAppFixture>(
    ITestOutputHelper outputHelper,
    TAppFixture rocketSurgeryWebAppFixture) : LoggerTest<XUnitTestContext>(XUnitTestContext.Create(outputHelper)), IClassFixture<TAppFixture>, IAsyncLifetime
    where TAppFixture : class, ILaunchPadWebAppFixture
{
    protected IAlbaHost AlbaHost => rocketSurgeryWebAppFixture.AlbaHost;

    /// <summary>
    ///     The Service Provider
    /// </summary>
    protected IServiceProvider ServiceProvider => AlbaHost.Services;

    public virtual Task InitializeAsync()
    {
        rocketSurgeryWebAppFixture.SetLoggerFactory(CreateLoggerFactory());
        return rocketSurgeryWebAppFixture.ResetAsync();
    }

    public virtual Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
