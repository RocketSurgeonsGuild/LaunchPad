using Alba;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Serilog.Events;

namespace Sample.Graphql.Tests.Helpers;
public abstract class GraphQlWebAppFixtureTest<TAppFixture>(ITestOutputHelper outputHelper, TAppFixture rocketSurgeryWebAppFixture, LogEventLevel logEventLevel = LogEventLevel.Verbose) : LoggerTest<XUnitTestContext>(XUnitTestContext.Create(outputHelper, logEventLevel)), IClassFixture<TAppFixture>, IAsyncLifetime
    where TAppFixture : class, ILaunchPadWebAppFixture
{
    protected IAlbaHost Host => rocketSurgeryWebAppFixture.AlbaHost;

    /// <summary>
    ///     The Service Provider
    /// </summary>
    protected IServiceProvider ServiceProvider => Host.Services;

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
