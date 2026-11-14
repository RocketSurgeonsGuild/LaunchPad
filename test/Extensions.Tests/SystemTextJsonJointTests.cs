using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.Foundation.Joints;

namespace Extensions.Tests;

public class SystemTextJsonJointTests
{
    // The package's generator registers a camel case JsonStringEnumConverter<TEnum> for every enum a JSON context reaches (TestJsonContext
    // reaches DayOfWeek and Stage), so no reflection based converter is needed.
    [Test]
    public async Task Writes_The_Enums_Of_Json_Contexts_As_Camel_Case_Strings()
    {
        var options = await Options(services => services.AddJsonTypeInfoResolver(TestJsonContext.Default));

        await JsonSerializer.Serialize(DayOfWeek.Monday, options).Should().BeEqualTo("\"monday\"");
        await JsonSerializer.Deserialize<DayOfWeek>("\"tuesday\"", options).Should().BeEqualTo(DayOfWeek.Tuesday);
        await JsonSerializer.Serialize(new TestRocket("falcon", 9, Stage.LowOrbit), options).Should().BeEqualTo("""{"name":"falcon","engines":9,"stage":"lowOrbit"}""");
        await options.Converters.Should().All(x => x.GetType() != typeof(JsonStringEnumConverter));
    }

    // An enum no JSON context reaches has no converter: with a reflection resolver (ASP.NET Core's options in a JIT application) it is a number.
    [Test]
    public async Task Writes_Other_Enums_As_Numbers()
    {
        var options = await Options(services => services.Configure<JsonSerializerOptions>(o => o.TypeInfoResolverChain.Add(new DefaultJsonTypeInfoResolver())));

        await JsonSerializer.Serialize(Unreached.Second, options).Should().BeEqualTo("1");
    }

    [Test]
    public async Task Uses_Camel_Case_Property_Names()
    {
        var options = await Options();

        await options.PropertyNamingPolicy.Should().BeEqualTo(JsonNamingPolicy.CamelCase);
    }

    // NodaTime support is its own package (NodaTimeSystemTextJsonJoint), so Foundation must not ask for a time zone provider.
    // Resolving the options runs every post configure registered for them, and one that wants a provider throws when none is registered.
    [Test]
    public async Task Does_Not_Need_A_Time_Zone_Provider() => await Options();

    [Test]
    public async Task Puts_Registered_Resolvers_In_Front_Of_The_Chain_And_Never_Adds_Reflection()
    {
        var options = await Options(services => services.AddJsonTypeInfoResolver(TestJsonContext.Default));

        await options.TypeInfoResolverChain[0].Should().BeSameReferenceAs(TestJsonContext.Default);
        await options.TypeInfoResolverChain.Should().All(x => x is not DefaultJsonTypeInfoResolver);
        await ( (Action)( () => JsonSerializer.Serialize(new UnknownRocket("starship"), options) ) ).Should().Throw<NotSupportedException>();
    }

    [Test]
    public async Task Keeps_A_Reflection_Resolver_The_Application_Adds()
    {
        var reflection = new DefaultJsonTypeInfoResolver();
        var options = await Options(services => services.Configure<JsonSerializerOptions>(o => o.TypeInfoResolverChain.Add(reflection)));

        await options.TypeInfoResolverChain[^1].Should().BeSameReferenceAs(reflection);
        await JsonSerializer.Serialize(new UnknownRocket("starship"), options).Should().BeEqualTo("""{"name":"starship"}""");
    }

    [Test]
    public async Task Orders_Resolvers_By_Their_Order_Then_By_Registration()
    {
        var first = new JsonConverterTypeInfoResolver();
        var second = new JsonConverterTypeInfoResolver();
        var library = new JsonConverterTypeInfoResolver();
        var options = await Options(
            services => services
                       .AddJsonTypeInfoResolver(library, JsonTypeInfoResolverRegistration.LibraryOrder)
                       .AddJsonTypeInfoResolver(first)
                       .AddJsonTypeInfoResolver(second)
                       .AddJsonTypeInfoResolver(first)
        );

        var chain = options.TypeInfoResolverChain.ToList();
        await chain.IndexOf(first).Should().BeEqualTo(0);
        await chain.IndexOf(second).Should().BeEqualTo(1);
        await chain.Count(x => ReferenceEquals(x, first)).Should().BeEqualTo(1);
        await chain.IndexOf(library).Should().BeGreaterThan(1);
    }

    [Test]
    public async Task Uses_The_Applications_Context_For_Its_Types()
    {
        var options = await Options(services => services.AddJsonTypeInfoResolver(TestJsonContext.Default));

        // TryGetTypeInfo rather than GetTypeInfo<T>(), which System.Text.Json has only from .NET 11.
        await options.TryGetTypeInfo(typeof(TestRocket), out var typeInfo).Should().BeTrue();
        await typeInfo!.OriginatingResolver.Should().BeSameReferenceAs(TestJsonContext.Default);
    }

    // Native AOT turns reflection off: then only the chained resolvers are left, and what LaunchPad writes as an object must still serialize.
    [Test]
    public async Task Serializes_LaunchPads_Object_Values_Without_Reflection()
    {
        var options = WithoutReflection(await Options());

        await JsonSerializer.Serialize<object>(new Dictionary<string, object> { ["count"] = 3, ["name"] = "falcon", ["tags"] = Tags }, options)
                      .Should().BeEqualTo("""{"count":3,"name":"falcon","tags":["a","b"]}""");
        await JsonSerializer.Serialize<object>(Guid.Empty, options).Should().BeEqualTo("\"00000000-0000-0000-0000-000000000000\"");
        await JsonSerializer.Serialize<object>(JsonNode.Parse("""{"a":1}""")!, options).Should().BeEqualTo("""{"a":1}""");
    }

    [Test]
    public async Task Serializes_The_Applications_Types_Without_Reflection()
    {
        var options = WithoutReflection(await Options(services => services.AddJsonTypeInfoResolver(TestJsonContext.Default)));

        await JsonSerializer.Serialize<object>(new TestRocket("falcon", 9, Stage.Pad), options).Should().BeEqualTo("""{"name":"falcon","engines":9,"stage":"pad"}""");
        await ( (Action)( () => JsonSerializer.Serialize<object>(new UnknownRocket("starship"), options) ) ).Should().Throw<NotSupportedException>();
    }

    [Test]
    public async Task Resolves_Types_Written_By_A_Converter_In_The_Options()
    {
        var options = WithoutReflection(
            await Options(
                services => services
                           .Configure<JsonSerializerOptions>(o => o.Converters.Add(new RocketNameConverter()))
                           .AddJsonTypeInfoResolver(new JsonConverterTypeInfoResolver().AddNullable<RocketName>())
            )
        );

        await JsonSerializer.Serialize<object>(new RocketName("falcon"), options).Should().BeEqualTo("\"falcon\"");
        await JsonSerializer.Serialize<RocketName?>(null, options).Should().BeEqualTo("null");
        await JsonSerializer.Deserialize<RocketName?>("\"starship\"", options).Should().BeEqualTo(new RocketName("starship"));
    }

    private static readonly string[] Tags = ["a", "b"];

    private static JsonSerializerOptions WithoutReflection(JsonSerializerOptions options) =>
        new(options) { TypeInfoResolver = JsonTypeInfoResolver.Combine(options.TypeInfoResolverChain.Where(x => x is not DefaultJsonTypeInfoResolver).ToArray()) };

    private static async Task<JsonSerializerOptions> Options(Action<IServiceCollection>? configure = null)
    {
        // AddOptions comes from the Foundation DefaultJoint in an application.
        var services = new ServiceCollection().AddOptions();
        new SystemTextJsonJoint().Register(await JointContext.Create(), services);
        configure?.Invoke(services);
        return services.BuildServiceProvider().GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
    }
}

public sealed record TestRocket(string Name, int Engines, Stage Stage);

public enum Stage
{
    Pad,
    LowOrbit,
}

public enum Unreached
{
    First,
    Second,
}

public sealed record UnknownRocket(string Name);

public readonly record struct RocketName(string Value);

public sealed class RocketNameConverter : JsonConverter<RocketName>
{
    public override RocketName Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => new(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, RocketName value, JsonSerializerOptions options) => writer.WriteStringValue(value.Value);
}

[JsonSerializable(typeof(TestRocket))]
[JsonSerializable(typeof(DayOfWeek))]
public partial class TestJsonContext : JsonSerializerContext;
