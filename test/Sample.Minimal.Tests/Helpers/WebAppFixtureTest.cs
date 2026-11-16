using Alba;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Refit;

using TUnit.Core.Interfaces;

namespace Sample.Minimal.Tests.Helpers;

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
    ///     The Refit client Chirograph generates from the sample's OpenAPI document, registered with its generated
    ///     <c>AddSampleClient()</c> and sent to the test server.
    /// </summary>
    /// <remarks>
    ///     Properties without a value are left out of the requests: the sample's patch requests treat a property that is sent as
    ///     <see langword="null" /> as a change, where an <c>Assigned</c> value it doesn't send is left alone.
    /// </remarks>
    protected ISampleClient Client => field ??= CreateClient();

    private ISampleClient CreateClient()
    {
        var services = new ServiceCollection();
        // the client's address, under the section the generated registration reads (Clients:SampleClient)
        services.AddSingleton<Microsoft.Extensions.Configuration.IConfiguration>(
            new ConfigurationBuilder()
               .AddInMemoryCollection([new("Clients:SampleClient:BaseAddress", AlbaHost.Server.BaseAddress.ToString())])
               .Build()
        );
        services.AddSingleton(
            new RefitSettings(
                new SystemTextJsonContentSerializer(
                    new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web)
                    {
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                    }
                )
            )
        );
        // the project references Microsoft.Extensions.ServiceDiscovery (through LaunchPad's hosting), so the generated registration resolves
        // its address through service discovery, which needs a provider: the pass-through one sends the BaseAddress as it is
        services.AddServiceDiscovery();
        services
           .AddSampleClient()
           .ConfigurePrimaryHttpMessageHandler(AlbaHost.Server.CreateHandler);
        return services.BuildServiceProvider().GetRequiredService<ISampleClient>();
    }

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
