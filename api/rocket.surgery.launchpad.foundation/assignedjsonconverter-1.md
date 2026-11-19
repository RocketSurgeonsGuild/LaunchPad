# AssignedJsonConverter<T>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-beta.413`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.5756d1abb55aa6348e0f70f98329f405a8d0c1f6/src/Foundation/AssignedJsonConverter.cs)
- Inherits: `JsonConverter<Assigned<T>>`

Reads and writes an `Foundation.Assigned`1` as the bare `T`: a property that is in the JSON is assigned its value, one that isn't stays unassigned (the property's own `Assigned`1.Empty`).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public sealed class AssignedJsonConverter<T>
    : System.Text.Json.Serialization.JsonConverter<Rocket.Surgery.LaunchPad.Foundation.Assigned<T>>
{
    // ...
}
```

## Type Parameters

- `T` -- The type of the value.

## Remarks

An explicit `null` assigns `null` when `T` can hold it (a reference type or a `Nullable`1`): that is how a client clears a value, and a request whose property can't be null rejects it when it is validated. For a value type that can't be null (an enum, a number) it leaves the property unassigned, as if it weren't sent: it has no value to clear to, and client generators write such an optional property as nullable and send `null` for one that wasn't set.

The property tracking generator puts it on every `Foundation.Assigned`1` property it writes, closed over the property's type, so no converter is made at runtime and it is safe for trimming and native AOT.

## Constructors

- [AssignedJsonConverter](/LaunchPad/api/rocket.surgery.launchpad.foundation/assignedjsonconverter-1/constructors.md#constructor)

## Properties

- [HandleNull](/LaunchPad/api/rocket.surgery.launchpad.foundation/assignedjsonconverter-1/properties.md#handlenull) : `bool` `get` -- Gets a value that indicates whether `null` should be passed to the converter on serialization, and whether `JsonTokenType.Null` should be passed on deserialization.

## Methods

- [Read(Utf8JsonReader, Type, JsonSerializerOptions)](/LaunchPad/api/rocket.surgery.launchpad.foundation/assignedjsonconverter-1/methods.md#read-utf8jsonreader-type-jsonserializeroptions) : [Assigned<T>](/LaunchPad/api/rocket.surgery.launchpad.foundation/assigned-1.md) -- Reads and converts the JSON to type `Foundation.Assigned{`0}`.
- [Write(Utf8JsonWriter, Assigned<T>, JsonSerializerOptions)](/LaunchPad/api/rocket.surgery.launchpad.foundation/assignedjsonconverter-1/methods.md#write-utf8jsonwriter-assigned-t-jsonserializeroptions) -- Writes a specified value as JSON.
