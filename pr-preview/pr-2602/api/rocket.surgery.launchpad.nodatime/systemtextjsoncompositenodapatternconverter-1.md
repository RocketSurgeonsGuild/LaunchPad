# SystemTextJsonCompositeNodaPatternConverter<T>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.nodatime.md)
- Version: `2.0.1-pr.1259`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-2602-merge.Sha.f387c871fa01cb1c7bfc78f0117912dc30d931f2/src/NodaTime/SystemTextJsonCompositeNodaPatternConverter.cs)
- Inherits: `JsonConverter<T>`

A JSON converter for types which can be represented by a single string value, parsed or formatted from an `Text.IPattern`1`.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.NodaTime;

public sealed class SystemTextJsonCompositeNodaPatternConverter<T>
    : System.Text.Json.Serialization.JsonConverter<T>
{
    // ...
}
```

## Type Parameters

- `T` -- The type to convert to/from JSON.

## Remarks

Creates a new instance with a pattern and an optional validator. The validator will be called before each value is written, and may throw an exception to indicate that the value cannot be serialized.

The first pattern formats; reading tries each pattern in turn. The value is also read and written as a property name, so the type can be a dictionary key. A value no pattern parses throws `Json.JsonException`.

## Constructors

- [SystemTextJsonCompositeNodaPatternConverter(Action<T>, IPattern<T>)](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.nodatime/systemtextjsoncompositenodapatternconverter-1/constructors.md#constructor-action-t-ipattern-t) -- A JSON converter for types which can be represented by a single string value, parsed or formatted from an `Text.IPattern`1`.
- [SystemTextJsonCompositeNodaPatternConverter(IPattern<T>)](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.nodatime/systemtextjsoncompositenodapatternconverter-1/constructors.md#constructor-ipattern-t) -- Creates a new instance with a pattern and no validator.

## Methods

- [Read(Utf8JsonReader, Type, JsonSerializerOptions)](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.nodatime/systemtextjsoncompositenodapatternconverter-1/methods.md#read-utf8jsonreader-type-jsonserializeroptions) : `T` -- Reads and converts the JSON to type `T`.
- [ReadAsPropertyName(Utf8JsonReader, Type, JsonSerializerOptions)](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.nodatime/systemtextjsoncompositenodapatternconverter-1/methods.md#readaspropertyname-utf8jsonreader-type-jsonserializeroptions) : `T` -- Reads a dictionary key from a JSON property name.
- [Write(Utf8JsonWriter, T, JsonSerializerOptions)](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.nodatime/systemtextjsoncompositenodapatternconverter-1/methods.md#write-utf8jsonwriter-t-jsonserializeroptions) -- Writes a specified value as JSON.
- [WriteAsPropertyName(Utf8JsonWriter, T, JsonSerializerOptions)](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.nodatime/systemtextjsoncompositenodapatternconverter-1/methods.md#writeaspropertyname-utf8jsonwriter-t-jsonserializeroptions) -- Writes a dictionary key as a JSON property name.
