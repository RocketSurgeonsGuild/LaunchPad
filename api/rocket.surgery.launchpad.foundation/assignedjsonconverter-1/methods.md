# AssignedJsonConverter<T> Methods

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/api/rocket.surgery.launchpad.foundation.md)
- Type: [AssignedJsonConverter<T>](/LaunchPad/api/rocket.surgery.launchpad.foundation/assignedjsonconverter-1.md)
- Kind: `Methods`
- Members: `2`

Reads and writes an `Foundation.Assigned`1` as the bare `T`: a property that is in the JSON is assigned its value, one that isn't stays unassigned (the property's own `Assigned`1.Empty`).

<a id="read"></a>
<a id="read-utf8jsonreader-type-jsonserializeroptions"></a>

## Read(Utf8JsonReader, Type, JsonSerializerOptions)

- Name: `Read(Utf8JsonReader, Type, JsonSerializerOptions)`
- Modifiers: `override`
- Returns: [Assigned<T>](/LaunchPad/api/rocket.surgery.launchpad.foundation/assigned-1.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.7c2af63fb1eca0ecbe24e62d859d0132ae74c3a6/src/Foundation/AssignedJsonConverter.cs#L33-L36)

Reads and converts the JSON to type `Foundation.Assigned{`0}`.

```csharp
public sealed class AssignedJsonConverter<T>
{
    public override Assigned<T> Read(
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

[Assigned<T>](/LaunchPad/api/rocket.surgery.launchpad.foundation/assigned-1.md) -- The converted value.

<a id="write"></a>
<a id="write-utf8jsonwriter-assigned-t-jsonserializeroptions"></a>

## Write(Utf8JsonWriter, Assigned<T>, JsonSerializerOptions)

- Name: `Write(Utf8JsonWriter, Assigned<T>, JsonSerializerOptions)`
- Modifiers: `override`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.7c2af63fb1eca0ecbe24e62d859d0132ae74c3a6/src/Foundation/AssignedJsonConverter.cs#L42-L51)

Writes a specified value as JSON.

```csharp
public sealed class AssignedJsonConverter<T>
{
    public override void Write(
        Utf8JsonWriter writer,
        Assigned<T> value,
        JsonSerializerOptions options)
    {
        // ...
    }
}
```

## Parameters

- `writer` (`Utf8JsonWriter`)
  The writer to write to.
- `value` ([Assigned<T>](/LaunchPad/api/rocket.surgery.launchpad.foundation/assigned-1.md))
  The value to convert to JSON.
- `options` (`JsonSerializerOptions`)
  An object that specifies serialization options to use.
