# JsonConverterTypeInfoResolver Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3173/api/rocket.surgery.launchpad.primitives.md)
- Type: [JsonConverterTypeInfoResolver](/LaunchPad/pr-preview/pr-3173/api/rocket.surgery.launchpad.primitives/jsonconvertertypeinforesolver.md)
- Kind: `Methods`
- Members: `3`

Resolves the metadata of value types written by a converter in `JsonSerializerOptions.Converters`, such as the NodaTime or the spatial types, without reflection.

<a id="add"></a>

## Add

- Name: `Add`
- Returns: [JsonConverterTypeInfoResolver](/LaunchPad/pr-preview/pr-3173/api/rocket.surgery.launchpad.primitives/jsonconvertertypeinforesolver.md)

Resolves `T` with the converter for it in the options.

```csharp
public sealed class JsonConverterTypeInfoResolver
{
    public JsonConverterTypeInfoResolver Add<T>()
    {
        // ...
    }
}
```

## Returns

[JsonConverterTypeInfoResolver](/LaunchPad/pr-preview/pr-3173/api/rocket.surgery.launchpad.primitives/jsonconvertertypeinforesolver.md) -- This resolver.

<a id="addnullable"></a>

## AddNullable

- Name: `AddNullable`
- Returns: [JsonConverterTypeInfoResolver](/LaunchPad/pr-preview/pr-3173/api/rocket.surgery.launchpad.primitives/jsonconvertertypeinforesolver.md)

Resolves `T` and `Nullable`1` with the converter for `T` in the options.

```csharp
public sealed class JsonConverterTypeInfoResolver
{
    public JsonConverterTypeInfoResolver AddNullable<T>()
    {
        // ...
    }
}
```

## Returns

[JsonConverterTypeInfoResolver](/LaunchPad/pr-preview/pr-3173/api/rocket.surgery.launchpad.primitives/jsonconvertertypeinforesolver.md) -- This resolver.

<a id="gettypeinfo"></a>
<a id="gettypeinfo-type-jsonserializeroptions"></a>

## GetTypeInfo(Type, JsonSerializerOptions)

- Name: `GetTypeInfo(Type, JsonSerializerOptions)`
- Modifiers: `nullable`
- Returns: `JsonTypeInfo?`

Resolves a `Metadata.JsonTypeInfo` contract for the requested type and options.

```csharp
public sealed class JsonConverterTypeInfoResolver
{
    public JsonTypeInfo? GetTypeInfo(
        Type type,
        JsonSerializerOptions options)
    {
        // ...
    }
}
```

## Parameters

- `type` (`Type`)
  Type to be resolved.
- `options` (`JsonSerializerOptions`)
  Configuration used when resolving the metadata.

## Returns

`JsonTypeInfo?` -- A `Metadata.JsonTypeInfo` instance matching the requested type, or `null` if no contract could be resolved.
