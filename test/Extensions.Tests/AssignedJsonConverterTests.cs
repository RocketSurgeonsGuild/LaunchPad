using System.Text.Json;
using System.Text.Json.Serialization;

using Rocket.Surgery.LaunchPad.Foundation;

namespace Extensions.Tests;

public class AssignedJsonConverterTests
{
    [Test]
    public async Task Should_Read_A_Present_Value_As_Assigned()
    {
        var patch = Read("""{"name":"Falcon","count":3}""");

        await patch.Name.HasValue.Should().BeTrue();
        await patch.Name.Value.Should().BeEqualTo("Falcon");
        await patch.Count.HasValue.Should().BeTrue();
        await patch.Count.Value.Should().BeEqualTo(3);
    }

    [Test]
    public async Task Should_Leave_An_Absent_Property_Unassigned()
    {
        var patch = Read("{}");

        await patch.Name.HasValue.Should().BeFalse();
        await patch.Count.HasValue.Should().BeFalse();
        await patch.Cleared.HasValue.Should().BeFalse();
    }

    [Test]
    public async Task Should_Assign_Null_To_A_Type_That_Can_Hold_It()
    {
        var patch = Read("""{"name":null,"cleared":null}""");

        await patch.Name.HasValue.Should().BeTrue();
        await patch.Name.Value.Should().BeNull();
        await patch.Cleared.HasValue.Should().BeTrue();
        await patch.Cleared.Value.Should().BeNull();
    }

    [Test]
    public async Task Should_Leave_A_Value_Type_Unassigned_When_It_Is_Null()
    {
        var patch = Read("""{"count":null}""");

        await patch.Count.HasValue.Should().BeFalse();
    }

    [Test]
    public async Task Should_Write_The_Bare_Value()
    {
        var json = JsonSerializer.Serialize(new Patch { Name = "Falcon", Count = 3, Cleared = new Assigned<int?>(null) }, Options);

        await json.Should().BeEqualTo("""{"name":"Falcon","count":3,"cleared":null}""");
    }

    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    private static Patch Read(string json) => JsonSerializer.Deserialize<Patch>(json, Options)!;

    private sealed class Patch
    {
        [JsonConverter(typeof(AssignedJsonConverter<string>))]
        public Assigned<string> Name { get; set; } = Assigned<string>.Empty();

        [JsonConverter(typeof(AssignedJsonConverter<int>))]
        public Assigned<int> Count { get; set; } = Assigned<int>.Empty();

        [JsonConverter(typeof(AssignedJsonConverter<int?>))]
        public Assigned<int?> Cleared { get; set; } = Assigned<int?>.Empty();
    }
}
