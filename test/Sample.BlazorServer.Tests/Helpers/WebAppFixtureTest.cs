using Alba;

using Rocket.Surgery.LaunchPad.AspNetCore.Testing;

namespace Sample.BlazorServer.Tests.Helpers;

public abstract class WebAppFixtureTest<TAppFixture>
    (ITestContextAccessor outputHelper, TAppFixture rocketSurgeryWebAppFixture)
    : LoggerTest<XUnitTestContext>(XUnitDefaults.CreateTestContext(outputHelper)), IClassFixture<TAppFixture>, IAsyncLifetime
    where TAppFixture : class, ILaunchPadWebAppFixture
{
    public virtual ValueTask InitializeAsync()
    {
        rocketSurgeryWebAppFixture.SetLoggerFactory(CreateLoggerFactory());
        return rocketSurgeryWebAppFixture.ResetAsync();
    }

    public virtual ValueTask DisposeAsync() => ValueTask.CompletedTask;
    protected IAlbaHost AlbaHost => rocketSurgeryWebAppFixture.AlbaHost;

    /// <summary>
    ///     The Service Provider
    /// </summary>
    protected IServiceProvider ServiceProvider => AlbaHost.Services;
}
