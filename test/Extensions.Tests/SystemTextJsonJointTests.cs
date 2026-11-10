using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.Primitives;
using Rocket.Surgery.LaunchPad.Primitives.Joints;

namespace Extensions.Tests;

public class SystemTextJsonJointTests
{
    // The package's generator registers a camel case JsonStringEnumConverter<TEnum> for every enum a JSON context reaches (TestJsonContext
    // reaches DayOfWeek and Stage), so no reflection based converter is needed.
    [Test]
    public async Task Writes_The_Enums_Of_Json_Contexts_As_Camel_Case_Strings()
    {
        var options = await Options(services => services.AddJsonTypeInfoResolver(TestJsonContext.Default));

        JsonSerializer.Serialize(DayOfWeek.Monday, options).ShouldBe("\"monday\"");
        JsonSerializer.Deserialize<DayOfWeek>("\"tuesday\"", options).ShouldBe(DayOfWeek.Tuesday);
        JsonSerializer.Serialize(new TestRocket("falcon", 9, Stage.LowOrbit), options).ShouldBe("""{"name":"falcon","engines":9,"stage":"lowOrbit"}""");
        options.Converters.ShouldNotContain(x => x.GetType() == typeof(JsonStringEnumConverter));
    }

    // An enum no JSON context reaches has no converter: with a reflection resolver (ASP.NET Core's options in a JIT application) it is a number.
    [Test]
    public async Task Writes_Other_Enums_As_Numbers()
    {
        var options = await Options(services => services.Configure<JsonSerializerOptions>(o => o.TypeInfoResolverChain.Add(new DefaultJsonTypeInfoResolver())));

        JsonSerializer.Serialize(Unreached.Second, options).ShouldBe("1");
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

    [Test]
    public async Task Puts_Registered_Resolvers_In_Front_Of_The_Chain_And_Never_Adds_Reflection()
    {
        var options = await Options(services => services.AddJsonTypeInfoResolver(TestJsonContext.Default));

        options.TypeInfoResolverChain[0].ShouldBeSameAs(TestJsonContext.Default);
        options.TypeInfoResolverChain.ShouldNotContain(x => x is DefaultJsonTypeInfoResolver);
        Should.Throw<NotSupportedException>(() => JsonSerializer.Serialize(new UnknownRocket("starship"), options));
    }

    [Test]
    public async Task Keeps_A_Reflection_Resolver_The_Application_Adds()
    {
        var reflection = new DefaultJsonTypeInfoResolver();
        var options = await Options(services => services.Configure<JsonSerializerOptions>(o => o.TypeInfoResolverChain.Add(reflection)));

        options.TypeInfoResolverChain[^1].ShouldBeSameAs(reflection);
        JsonSerializer.Serialize(new UnknownRocket("starship"), options).ShouldBe("""{"name":"starship"}""");
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
        chain.IndexOf(first).ShouldBe(0);
        chain.IndexOf(second).ShouldBe(1);
        chain.Count(x => ReferenceEquals(x, first)).ShouldBe(1);
        chain.IndexOf(library).ShouldBeGreaterThan(1);
    }

    [Test]
    public async Task Uses_The_Applications_Context_For_Its_Types()
    {
        var options = await Options(services => services.AddJsonTypeInfoResolver(TestJsonContext.Default));

        // TryGetTypeInfo rather than GetTypeInfo<T>(), which System.Text.Json has only from .NET 11.
        options.TryGetTypeInfo(typeof(TestRocket), out var typeInfo).ShouldBeTrue();
        typeInfo!.OriginatingResolver.ShouldBeSameAs(TestJsonContext.Default);
    }

    // Native AOT turns reflection off: then only the chained resolvers are left, and what LaunchPad writes as an object must still serialize.
    [Test]
    public async Task Serializes_LaunchPads_Object_Values_Without_Reflection()
    {
        var options = WithoutReflection(await Options());

        JsonSerializer.Serialize<object>(new Dictionary<string, object> { ["count"] = 3, ["name"] = "falcon", ["tags"] = Tags }, options)
                      .ShouldBe("""{"count":3,"name":"falcon","tags":["a","b"]}""");
        JsonSerializer.Serialize<object>(Guid.Empty, options).ShouldBe("\"00000000-0000-0000-0000-000000000000\"");
        JsonSerializer.Serialize<object>(JsonNode.Parse("""{"a":1}""")!, options).ShouldBe("""{"a":1}""");
    }

    [Test]
    public async Task Serializes_The_Applications_Types_Without_Reflection()
    {
        var options = WithoutReflection(await Options(services => services.AddJsonTypeInfoResolver(TestJsonContext.Default)));

        JsonSerializer.Serialize<object>(new TestRocket("falcon", 9, Stage.Pad), options).ShouldBe("""{"name":"falcon","engines":9,"stage":"pad"}""");
        Should.Throw<NotSupportedException>(() => JsonSerializer.Serialize<object>(new UnknownRocket("starship"), options));
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

        JsonSerializer.Serialize<object>(new RocketName("falcon"), options).ShouldBe("\"falcon\"");
        JsonSerializer.Serialize<RocketName?>(null, options).ShouldBe("null");
        JsonSerializer.Deserialize<RocketName?>("\"starship\"", options).ShouldBe(new RocketName("starship"));
    }

    private static readonly string[] Tags = ["a", "b"];

    private static JsonSerializerOptions WithoutReflection(JsonSerializerOptions options) =>
        new(options) { TypeInfoResolver = JsonTypeInfoResolver.Combine(options.TypeInfoResolverChain.Where(x => x is not DefaultJsonTypeInfoResolver).ToArray()) };

    private static async Task<JsonSerializerOptions> Options(Action<IServiceCollection>? configure = null)
    {
        // AddOptions comes from the Primitives DefaultJoint in an application.
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
