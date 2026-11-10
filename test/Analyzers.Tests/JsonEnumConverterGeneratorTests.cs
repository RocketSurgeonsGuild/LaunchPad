using Rocket.Surgery.LaunchPad.Analyzers;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Analyzers.Tests;

public class JsonEnumConverterGeneratorTests() : GeneratorTest()
{
    // A stand-in for a source generated context: the generator reads the [JsonSerializable] attributes, so the System.Text.Json source generator
    // doesn't need to run, and an abstract class compiles without the members it would write.
    private const string Source = """
        using System.Collections.Generic;
        using System.Text.Json.Serialization;

        namespace Sample.Contracts
        {
            public enum RocketType { Falcon, Starship }
            public enum LaunchStatus { Planned, Scrubbed }
            public enum Unreached { One }

            [JsonConverter(typeof(JsonNumberEnumConverter<Engine>))]
            public enum Engine { Merlin, Raptor }

            public record LaunchRecord(LaunchStatus? Status, Engine Engine);
            public record RocketModel(string Name, RocketType Type, List<LaunchRecord> Launches);

            [JsonSerializable(typeof(RocketModel))]
            [JsonSerializable(typeof(System.DayOfWeek))]
            public abstract partial class RocketsJsonContext : JsonSerializerContext
            {
                protected RocketsJsonContext() : base(null) { }
            }
        }
        """;

    [Test]
    public async Task Should_Register_The_Enums_A_Json_Context_Reaches()
    {
        var result = await Builder
                          .WithGenerator<JsonEnumConverterGenerator>()
                          .AddReferences(typeof(System.Text.Json.Serialization.JsonSerializerContext))
                          .AddReferences(typeof(LaunchPadJsonEnumConverters))
                          .AddSources(Source)
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result).UseFileName($"{nameof(JsonEnumConverterGeneratorTests)}.Should_Register_The_Enums_A_Json_Context_Reaches");
    }

    [Test]
    public async Task Should_Generate_Nothing_Without_LaunchPad_Foundation()
    {
        var result = await Builder
                          .WithGenerator<JsonEnumConverterGenerator>()
                          .AddReferences(typeof(System.Text.Json.Serialization.JsonSerializerContext))
                          .AddSources(Source)
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result).UseFileName($"{nameof(JsonEnumConverterGeneratorTests)}.Should_Generate_Nothing_Without_LaunchPad_Foundation");
    }
}
