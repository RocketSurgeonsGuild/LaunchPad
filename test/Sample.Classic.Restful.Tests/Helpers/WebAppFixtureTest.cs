using Alba;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Serilog;
using Serilog.Events;

namespace Sample.Classic.Restful.Tests.Helpers;

public abstract class WebAppFixtureTest<TAppFixture>
    (ITestOutputHelper outputHelper, TAppFixture rocketSurgeryWebAppFixture)
    : LoggerTest<XUnitTestContext>(XUnitTestContext.Create(outputHelper)), IClassFixture<TAppFixture>, IAsyncLifetime
    where TAppFixture : class, ILaunchPadWebAppFixture
{
    private readonly ILaunchPadWebAppFixture _rocketSurgeryWebAppFixture = rocketSurgeryWebAppFixture;
    protected IAlbaHost AlbaHost => _rocketSurgeryWebAppFixture.AlbaHost;

    /// <summary>
    ///     The Service Provider
    /// </summary>
    protected IServiceProvider ServiceProvider => AlbaHost.Services;

    public virtual Task InitializeAsync()
    {
        _rocketSurgeryWebAppFixture.SetLoggerFactory(CreateLoggerFactory());
        return _rocketSurgeryWebAppFixture.ResetAsync();
    }

    public virtual Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
