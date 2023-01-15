using Alba;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Serilog;
using Serilog.Events;

namespace Sample.Graphql.Tests.Helpers;

public class GraphQlWebAppFixtureTest<TAppFixture> : LoggerTest, IClassFixture<TAppFixture>, IAsyncLifetime
    where TAppFixture : class, ILaunchPadWebAppFixture
{
    private readonly ILaunchPadWebAppFixture _rocketSurgeryWebAppFixture;
    protected IAlbaHost Host => _rocketSurgeryWebAppFixture.AlbaHost;

    /// <summary>
    ///     The Service Provider
    /// </summary>
    protected IServiceProvider ServiceProvider => Host.Services;

    protected GraphQlWebAppFixtureTest(
        ITestOutputHelper outputHelper,
        TAppFixture rocketSurgeryWebAppFixture,
        string? logFormat = null,
        Action<LoggerConfiguration>? configureLogger = null
    ) : base(outputHelper, logFormat, configureLogger)
    {
        _rocketSurgeryWebAppFixture = rocketSurgeryWebAppFixture;
    }

    protected GraphQlWebAppFixtureTest(
        ITestOutputHelper outputHelper,
        TAppFixture rocketSurgeryWebAppFixture,
        LogLevel minLevel,
        string? logFormat = null,
        Action<LoggerConfiguration>? configureLogger = null
    ) : base(outputHelper, minLevel, logFormat, configureLogger)
    {
        _rocketSurgeryWebAppFixture = rocketSurgeryWebAppFixture;
    }

    protected GraphQlWebAppFixtureTest(
        ITestOutputHelper outputHelper,
        TAppFixture rocketSurgeryWebAppFixture,
        LogEventLevel minLevel,
        string? logFormat = null,
        Action<LoggerConfiguration>? configureLogger = null
    ) : base(outputHelper, minLevel, logFormat, configureLogger)
    {
        _rocketSurgeryWebAppFixture = rocketSurgeryWebAppFixture;
    }

    public virtual Task InitializeAsync()
    {
        _rocketSurgeryWebAppFixture.SetLoggerFactory(LoggerFactory);
        return _rocketSurgeryWebAppFixture.ResetAsync();
    }

    public virtual Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
