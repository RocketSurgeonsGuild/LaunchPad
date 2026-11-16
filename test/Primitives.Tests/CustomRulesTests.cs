using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using Rocket.Surgery.LaunchPad.StrongTypes;

using SuperStrong.Types;
using SuperStrong.Types.Converters;

using static Primitives.Tests.StrongTypeAssert;

namespace Primitives.Tests;

/// <summary>An application's own strong type built on a ready-made rules set: SuperStrong's generator reaches it through Primitives.</summary>
[StrongType<string, EmailAddressRules>]
public sealed partial class WorkEmail;

/// <summary>An application's own rules set, the example of docs/src/content/docs/concepts/strong-types.md.</summary>
public sealed class SkuRules : IStrongTypeTemplate<string>
{
    public static StrongTypeDefinition<string> Define() => StrongType
                                                         .Define<string>()
                                                         .HasLength(8)
                                                         .MatchesRegex("^[A-Z]{3}-[0-9]{4}$")
                                                         .Satisfies(value => !value.StartsWith("TST", StringComparison.Ordinal), "Test SKUs are not allowed.");
}

[StrongType<string, SkuRules>]
public sealed partial class Sku;

public class CustomRulesTests
{
    [Test]
    public async Task An_Application_Rules_Set_Backs_A_Type()
    {
        await Accepts<Sku, string>("ABC-1234");
        await Rejects<Sku, string>("TST-1234");
        await Rejects<Sku, string>("abc-1234");
        await Rejects<Sku, string>("ABC-12345");
    }

    [Test]
    public async Task An_Application_Type_Gets_The_Rules()
    {
        await Accepts<WorkEmail, string>("someone@example.com");
        await Rejects<WorkEmail, string>("not an email");
        await Rejects<WorkEmail, string>("someone@localhost");
    }

    [Test]
    public async Task An_Application_Type_Holds_The_Same_Value() =>
        await WorkEmail.From("someone@example.com").AsPrimitive().Should().BeEqualTo(EmailAddress.From("someone@example.com").AsPrimitive());

    // The System.Text.Json generator does not see the [JsonConverter] SuperStrong generates in the same compilation, so a JSON context in
    // the project that declares the strong type cannot list it; the converter is registered through a resolver instead.
    [Test]
    public async Task An_Application_Type_Round_Trips_Through_Json_With_Its_Converter()
    {
        var options = new JsonSerializerOptions(StrongTypesJsonContext.Default.Options)
        {
            TypeInfoResolver = JsonTypeInfoResolver.Combine(
                StrongTypesJsonContext.Default,
                new ConverterResolver<WorkEmail>(new JsonStrongTypeConverter<WorkEmail, string>())
            ),
        };
        var typeInfo = options.GetTypeInfo<WorkEmail>();

        await JsonSerializer.Serialize(WorkEmail.From("someone@example.com"), typeInfo).Should().BeEqualTo("\"someone@example.com\"");
        await JsonSerializer.Deserialize("\"someone@example.com\"", typeInfo).Should().BeEqualTo(WorkEmail.From("someone@example.com"));
        await ( (Action)( () => JsonSerializer.Deserialize("\"nope\"", typeInfo) ) ).Should().Throw<JsonException>();
    }

    private sealed class ConverterResolver<T>(JsonConverter<T> converter) : IJsonTypeInfoResolver
    {
        public JsonTypeInfo? GetTypeInfo(Type type, JsonSerializerOptions options) =>
            type == typeof(T) ? JsonMetadataServices.CreateValueInfo<T>(options, converter) : null;
    }
}
