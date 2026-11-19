# SystemTextJsonCompositeNodaPatternConverter<T> Methods

- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/api/rocket.surgery.launchpad.nodatime.md)
- Type: [SystemTextJsonCompositeNodaPatternConverter<T>](/LaunchPad/api/rocket.surgery.launchpad.nodatime/systemtextjsoncompositenodapatternconverter-1.md)
- Kind: `Methods`
- Members: `4`

A JSON converter for types which can be represented by a single string value, parsed or formatted from an `Text.IPattern`1`.

<a id="read"></a>
<a id="read-utf8jsonreader-type-jsonserializeroptions"></a>

## Read(Utf8JsonReader, Type, JsonSerializerOptions)

- Name: `Read(Utf8JsonReader, Type, JsonSerializerOptions)`
- Modifiers: `override`
- Returns: `T`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.5756d1abb55aa6348e0f70f98329f405a8d0c1f6/src/NodaTime/SystemTextJsonCompositeNodaPatternConverter.cs#L34)

Reads and converts the JSON to type `T`.

```csharp
public sealed class SystemTextJsonCompositeNodaPatternConverter<T>
{
    public override T Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        // ...
    }
}
```

## Parameters

- `reader` (`Utf8JsonReader`)
  The reader.
- `typeToConvert` (`Type`)
  The type to convert.
- `options` (`JsonSerializerOptions`)
  An object that specifies serialization options to use.

## Returns

`T` -- The converted value.

<a id="readaspropertyname"></a>
<a id="readaspropertyname-utf8jsonreader-type-jsonserializeroptions"></a>

## ReadAsPropertyName(Utf8JsonReader, Type, JsonSerializerOptions)

- Name: `ReadAsPropertyName(Utf8JsonReader, Type, JsonSerializerOptions)`
- Modifiers: `override`
- Returns: `T`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.5756d1abb55aa6348e0f70f98329f405a8d0c1f6/src/NodaTime/SystemTextJsonCompositeNodaPatternConverter.cs#L37)

Reads a dictionary key from a JSON property name.

```csharp
public sealed class SystemTextJsonCompositeNodaPatternConverter<T>
{
    public override T ReadAsPropertyName(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        // ...
    }
}
```

## Parameters

- `reader` (`Utf8JsonReader`)
  The `Json.Utf8JsonReader` to read from.
- `typeToConvert` (`Type`)
  The type to convert.
- `options` (`JsonSerializerOptions`)
  The options to use when reading the value.

## Returns

`T` -- The value that was converted.

<a id="write"></a>
<a id="write-utf8jsonwriter-t-jsonserializeroptions"></a>

## Write(Utf8JsonWriter, T, JsonSerializerOptions)

- Name: `Write(Utf8JsonWriter, T, JsonSerializerOptions)`
- Modifiers: `override`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.5756d1abb55aa6348e0f70f98329f405a8d0c1f6/src/NodaTime/SystemTextJsonCompositeNodaPatternConverter.cs#L42-L44)

Writes a specified value as JSON.

```csharp
public sealed class SystemTextJsonCompositeNodaPatternConverter<T>
{
    public override void Write(
        Utf8JsonWriter writer,
        T value,
        JsonSerializerOptions options)
    {
        // ...
    }
}
```

## Parameters

- `writer` (`Utf8JsonWriter`)
  The writer to write to.
- `value` (`T`)
  The value to convert to JSON.
- `options` (`JsonSerializerOptions`)
  An object that specifies serialization options to use.

<a id="writeaspropertyname"></a>
<a id="writeaspropertyname-utf8jsonwriter-t-jsonserializeroptions"></a>

## WriteAsPropertyName(Utf8JsonWriter, T, JsonSerializerOptions)

- Name: `WriteAsPropertyName(Utf8JsonWriter, T, JsonSerializerOptions)`
- Modifiers: `override`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.5756d1abb55aa6348e0f70f98329f405a8d0c1f6/src/NodaTime/SystemTextJsonCompositeNodaPatternConverter.cs#L49-L51)

Writes a dictionary key as a JSON property name.

```csharp
public sealed class SystemTextJsonCompositeNodaPatternConverter<T>
{
    public override void WriteAsPropertyName(
        Utf8JsonWriter writer,
        T value,
        JsonSerializerOptions options)
    {
        // ...
    }
}
```

## Parameters

- `writer` (`Utf8JsonWriter`)
  The `Json.Utf8JsonWriter` to write to.
- `value` (`T`)
  The value to convert. The value of `JsonConverter`1.HandleNull` determines if the converter handles `null` values.
- `options` (`JsonSerializerOptions`)
  The options to use when writing the value.
