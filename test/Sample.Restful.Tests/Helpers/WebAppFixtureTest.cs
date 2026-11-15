using Alba;

using Refit;

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
    ///     The Refit client the LaunchPad SDK generates from the sample's OpenAPI document, on the test server.
    /// </summary>
    /// <remarks>
    ///     Properties without a value are left out of the requests: the sample's patch requests treat a property that is sent as
    ///     <see langword="null" /> as a change, where an <c>Assigned</c> value it doesn't send is left alone.
    /// </remarks>
    protected ISampleClient Client =>
        RestService.ForGenerated<ISampleClient>(
            AlbaHost.Server.CreateClient(),
            new RefitSettings(
                new SystemTextJsonContentSerializer(
                    new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web)
                    {
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                    }
                )
            )
        );

    /// <summary>
    ///     Runs a request that has to fail, and returns its status and its problem details.
    /// </summary>
    protected static async Task<(System.Net.HttpStatusCode Status, TProblem? Problem)> Failure<TProblem>(Func<Task> request)
        where TProblem : class
    {
        try
        {
            await request();
        }
        catch (ApiException e)
        {
            return (e.StatusCode, await e.GetContentAsAsync<TProblem>());
        }

        throw new InvalidOperationException("The request succeeded");
    }

    /// <summary>
    ///     The Service Provider
    /// </summary>
    protected IServiceProvider ServiceProvider => AlbaHost.Services;
}
