using Alba;

using Serilog.Events;

using TUnit.Core.Interfaces;

namespace Sample.Graphql.Tests.Helpers;

/// <summary>
///     A test that runs against its own graphql web app.
/// </summary>
/// <remarks>
///     TUnit runs the tests of a class in parallel, and every test shares the in memory database of its web app, so
///     <see cref="SharedType.None" /> gives each test a fresh <see cref="GraphQlAppFixture" />. The fixture is started (and its
///     database created) before <see cref="InitializeAsync" /> runs, so there is nothing to reset.
/// </remarks>
public abstract class GraphQlWebAppFixtureTest(LogEventLevel logEventLevel = LogEventLevel.Verbose)
    : LoggerTest<TestRecord>(TestRecord.Create(logEventLevel)), IAsyncInitializer
{
    [ClassDataSource<GraphQlAppFixture>(Shared = SharedType.None)]
    public required GraphQlAppFixture WebAppFixture { get; init; }

    public virtual Task InitializeAsync()
    {
        WebAppFixture.SetLoggerFactory(CreateLoggerFactory());
        return Task.CompletedTask;
    }

    protected IAlbaHost Host => WebAppFixture.AlbaHost;

    /// <summary>
    ///     The Service Provider
    /// </summary>
    protected IServiceProvider ServiceProvider => Host.Services;
}
