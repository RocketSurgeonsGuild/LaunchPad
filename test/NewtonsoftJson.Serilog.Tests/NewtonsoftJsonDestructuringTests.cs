using Newtonsoft.Json.Linq;

using Rocket.Surgery.LaunchPad.Foundation;

using Serilog.Context;
using Serilog.Events;

namespace NewtonsoftJson.Serilog.Tests;

public class NewtonsoftJsonDestructuringTests : LoggerTest<TestRecord>
{
    [Test]
    public async Task Should_Destructure_NewtonsoftJson_JObject()
    {
        using var _ = CaptureLogs(out var logs);

        Logger.Information("This is just a test {@Data}", JObject.FromObject(new { test = true, system = new { data = "1234" } }));

        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Test]
    public async Task Should_Destructure_NewtonsoftJson_JArray()
    {
        using var _ = CaptureLogs(out var logs);

        Logger.Information("This is just a test {@Data}", JArray.FromObject(new object[] { 1, "2", 3d }));

        await Verify(logs.Select(z => z.RenderMessage()));
    }

    [Test]
    public async Task Should_Destructure_NewtonsoftJson_JValue()
    {
        var faker = new Faker
        {
            Random = new(17),
        };
        using var _ = CaptureLogs(out var logs);

        Logger.Information("This is just a test {@Data}", new JValue(faker.Random.Guid()));

        await Verify(logs.Select(z => z.RenderMessage()));
    }

    public NewtonsoftJsonDestructuringTests() : base(
        new(
            LogEventLevel.Information,
            configureLogger: (_, configuration) => configuration.Destructure.NewtonsoftJsonTypes()
        )
    ) => LogContext.PushProperty("SourceContext", nameof(NewtonsoftJsonDestructuringTests));
}
