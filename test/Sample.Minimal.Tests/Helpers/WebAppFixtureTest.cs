using Alba;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;

namespace Sample.Minimal.Tests.Helpers;

public abstract class WebAppFixtureTest<TAppFixture>(ITestContextAccessor outputHelper, TestWebAppFixture rocketSurgeryWebAppFixture) : LoggerTest<XUnitTestContext>(XUnitDefaults.CreateTestContext(outputHelper)), IClassFixture<TAppFixture>, IAsyncLifetime
    where TAppFixture : class, ILaunchPadWebAppFixture
{
    protected IAlbaHost AlbaHost => rocketSurgeryWebAppFixture.AlbaHost;

    /// <summary>
    ///     The Service Provider
    /// </summary>
    protected IServiceProvider ServiceProvider => AlbaHost.Services;

    public virtual ValueTask InitializeAsync()
    {
        rocketSurgeryWebAppFixture.SetLoggerFactory(CreateLoggerFactory());
        return rocketSurgeryWebAppFixture.ResetAsync();
    }

    public virtual ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
