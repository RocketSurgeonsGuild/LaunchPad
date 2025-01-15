using Alba;

using Rocket.Surgery.LaunchPad.AspNetCore.Testing;

namespace Sample.Classic.Restful.Tests.Helpers;

public abstract class WebAppFixtureTest<TAppFixture>
    (ITestOutputHelper outputHelper, TAppFixture rocketSurgeryWebAppFixture)
    : LoggerTest<XUnitTestContext>(XUnitTestContext.Create(outputHelper)), IClassFixture<TAppFixture>, IAsyncLifetime
    where TAppFixture : class, ILaunchPadWebAppFixture
{
    public virtual Task InitializeAsync()
    {
        _rocketSurgeryWebAppFixture.SetLoggerFactory(CreateLoggerFactory());
        return _rocketSurgeryWebAppFixture.ResetAsync();
    }

    public virtual Task DisposeAsync() => Task.CompletedTask;
    protected IAlbaHost AlbaHost => _rocketSurgeryWebAppFixture.AlbaHost;

    /// <summary>
    ///     The Service Provider
    /// </summary>
    protected IServiceProvider ServiceProvider => AlbaHost.Services;

    private readonly ILaunchPadWebAppFixture _rocketSurgeryWebAppFixture = rocketSurgeryWebAppFixture;
}
