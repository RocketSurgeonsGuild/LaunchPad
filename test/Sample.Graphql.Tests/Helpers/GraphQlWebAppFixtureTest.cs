using Alba;

using Rocket.Surgery.LaunchPad.AspNetCore.Testing;

using Serilog.Events;

namespace Sample.Graphql.Tests.Helpers;

public abstract class GraphQlWebAppFixtureTest<TAppFixture>
    (ITestContextAccessor outputHelper, TAppFixture rocketSurgeryWebAppFixture, LogEventLevel logEventLevel = LogEventLevel.Verbose)
    : LoggerTest<XUnitTestContext>(XUnitDefaults.CreateTestContext(outputHelper, logEventLevel)), IClassFixture<TAppFixture>, IAsyncLifetime
    where TAppFixture : class, ILaunchPadWebAppFixture
{
    public virtual ValueTask InitializeAsync()
    {
        rocketSurgeryWebAppFixture.SetLoggerFactory(CreateLoggerFactory());
        return rocketSurgeryWebAppFixture.ResetAsync();
    }

    public virtual ValueTask DisposeAsync() => ValueTask.CompletedTask;
    protected IAlbaHost Host => rocketSurgeryWebAppFixture.AlbaHost;

    /// <summary>
    ///     The Service Provider
    /// </summary>
    protected IServiceProvider ServiceProvider => Host.Services;
}
