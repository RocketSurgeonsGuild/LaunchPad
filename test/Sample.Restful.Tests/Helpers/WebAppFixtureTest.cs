using Alba;

using Rocket.Surgery.LaunchPad.AspNetCore.Testing;

namespace Sample.Restful.Tests.Helpers;

public abstract class WebAppFixtureTest<TAppFixture>
    (ITestOutputHelper outputHelper, TestWebAppFixture rocketSurgeryWebAppFixture) : LoggerTest<XUnitTestContext>(XUnitTestContext.Create(outputHelper)), IClassFixture<TAppFixture>, IAsyncLifetime
    where TAppFixture : class, ILaunchPadWebAppFixture
{
    public virtual Task InitializeAsync()
    {
        rocketSurgeryWebAppFixture.SetLoggerFactory(CreateLoggerFactory());
        return rocketSurgeryWebAppFixture.ResetAsync();
    }

    public virtual Task DisposeAsync() => Task.CompletedTask;
    protected IAlbaHost AlbaHost => rocketSurgeryWebAppFixture.AlbaHost;

    /// <summary>
    ///     The Service Provider
    /// </summary>
    protected IServiceProvider ServiceProvider => AlbaHost.Services;
}
