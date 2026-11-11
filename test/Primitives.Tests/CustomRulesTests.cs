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
    public void An_Application_Rules_Set_Backs_A_Type()
    {
        Accepts<Sku, string>("ABC-1234");
        Rejects<Sku, string>("TST-1234");
        Rejects<Sku, string>("abc-1234");
        Rejects<Sku, string>("ABC-12345");
    }

    [Test]
    public void An_Application_Type_Gets_The_Rules()
    {
        Accepts<WorkEmail, string>("someone@example.com");
        Rejects<WorkEmail, string>("not an email");
        Rejects<WorkEmail, string>("someone@localhost");
    }

    [Test]
    public void An_Application_Type_Holds_The_Same_Value() =>
        WorkEmail.From("someone@example.com").AsPrimitive().ShouldBe(EmailAddress.From("someone@example.com").AsPrimitive());

    // The System.Text.Json generator does not see the [JsonConverter] SuperStrong generates in the same compilation, so a JSON context in
    // the project that declares the strong type cannot list it; the converter is registered through a resolver instead.
    [Test]
    public void An_Application_Type_Round_Trips_Through_Json_With_Its_Converter()
    {
        var options = new JsonSerializerOptions(StrongTypesJsonContext.Default.Options)
        {
            TypeInfoResolver = JsonTypeInfoResolver.Combine(
                StrongTypesJsonContext.Default,
                new ConverterResolver<WorkEmail>(new JsonStrongTypeConverter<WorkEmail, string>())
            ),
        };
        // The generic GetTypeInfo<T> is new in .NET 11.
#pragma warning disable CA2263
        var typeInfo = (JsonTypeInfo<WorkEmail>)options.GetTypeInfo(typeof(WorkEmail));
#pragma warning restore CA2263

        JsonSerializer.Serialize(WorkEmail.From("someone@example.com"), typeInfo).ShouldBe("\"someone@example.com\"");
        JsonSerializer.Deserialize("\"someone@example.com\"", typeInfo).ShouldBe(WorkEmail.From("someone@example.com"));
        Should.Throw<JsonException>(() => JsonSerializer.Deserialize("\"nope\"", typeInfo));
    }

    private sealed class ConverterResolver<T>(JsonConverter<T> converter) : IJsonTypeInfoResolver
    {
        public JsonTypeInfo? GetTypeInfo(Type type, JsonSerializerOptions options) =>
            type == typeof(T) ? JsonMetadataServices.CreateValueInfo<T>(options, converter) : null;
    }
}
