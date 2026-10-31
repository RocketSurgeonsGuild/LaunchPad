using Alba;

using TUnit.Core.Interfaces;

namespace Sample.Restful.Tests.Helpers;

/// <summary>
///     A test that runs against its own web app.
/// </summary>
/// <remarks>
///     TUnit runs the tests of a class in parallel, and every test shares the in memory database of its web app, so
///     <see cref="SharedType.None" /> gives each test a fresh <see cref="TestWebAppFixture" />. The fixture is started (and its
///     database created) before <see cref="InitializeAsync" /> runs, so there is nothing to reset.
/// </remarks>
public abstract class WebAppFixtureTest() : LoggerTest<TestRecord>(TestRecord.Create()), IAsyncInitializer
{
    [ClassDataSource<TestWebAppFixture>(Shared = SharedType.None)]
    public required TestWebAppFixture WebAppFixture { get; init; }

    public virtual Task InitializeAsync()
    {
        WebAppFixture.SetLoggerFactory(CreateLoggerFactory());
        return Task.CompletedTask;
    }

    protected IAlbaHost AlbaHost => WebAppFixture.AlbaHost;

    /// <summary>
    ///     The Service Provider
    /// </summary>
    protected IServiceProvider ServiceProvider => AlbaHost.Services;
}
