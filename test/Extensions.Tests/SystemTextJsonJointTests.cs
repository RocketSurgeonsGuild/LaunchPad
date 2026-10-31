using System.Text.Json;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.Primitives.Joints;

namespace Extensions.Tests;

public class SystemTextJsonJointTests
{
    [Test]
    public async Task Writes_Enums_As_Camel_Case_Strings()
    {
        var options = await Options();

        JsonSerializer.Serialize(DayOfWeek.Monday, options).ShouldBe("\"monday\"");
        JsonSerializer.Deserialize<DayOfWeek>("\"tuesday\"", options).ShouldBe(DayOfWeek.Tuesday);
    }

    [Test]
    public async Task Uses_Camel_Case_Property_Names()
    {
        var options = await Options();

        options.PropertyNamingPolicy.ShouldBe(JsonNamingPolicy.CamelCase);
    }

    // NodaTime support is its own package (NodaTimeSystemTextJsonJoint), so Primitives must not ask for a time zone provider.
    // Resolving the options runs every post configure registered for them, and one that wants a provider throws when none is registered.
    [Test]
    public async Task Does_Not_Need_A_Time_Zone_Provider() => await Options();

    private static async Task<JsonSerializerOptions> Options()
    {
        // AddOptions comes from the Primitives DefaultJoint in an application.
        var services = new ServiceCollection().AddOptions();
        new SystemTextJsonJoint().Register(await JointContext.Create(), services);
        return services.BuildServiceProvider().GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
    }
}
