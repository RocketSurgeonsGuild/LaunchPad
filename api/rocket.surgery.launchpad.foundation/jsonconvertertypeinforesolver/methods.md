# JsonConverterTypeInfoResolver Methods

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/api/rocket.surgery.launchpad.foundation.md)
- Type: [JsonConverterTypeInfoResolver](/LaunchPad/api/rocket.surgery.launchpad.foundation/jsonconvertertypeinforesolver.md)
- Kind: `Methods`
- Members: `3`

Resolves the metadata of value types written by a converter in `JsonSerializerOptions.Converters`, such as the NodaTime or the spatial types, without reflection.

<a id="add"></a>

## Add

- Name: `Add`
- Returns: [JsonConverterTypeInfoResolver](/LaunchPad/api/rocket.surgery.launchpad.foundation/jsonconvertertypeinforesolver.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.54628ef221bd1ad09c6ebc55da16ddcb5027f4c7/src/Foundation/JsonConverterTypeInfoResolver.cs#L28-L29)

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

[JsonConverterTypeInfoResolver](/LaunchPad/api/rocket.surgery.launchpad.foundation/jsonconvertertypeinforesolver.md) -- This resolver.

<a id="addnullable"></a>

## AddNullable

- Name: `AddNullable`
- Returns: [JsonConverterTypeInfoResolver](/LaunchPad/api/rocket.surgery.launchpad.foundation/jsonconvertertypeinforesolver.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.54628ef221bd1ad09c6ebc55da16ddcb5027f4c7/src/Foundation/JsonConverterTypeInfoResolver.cs#L39-L41)

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

[JsonConverterTypeInfoResolver](/LaunchPad/api/rocket.surgery.launchpad.foundation/jsonconvertertypeinforesolver.md) -- This resolver.

<a id="gettypeinfo"></a>
<a id="gettypeinfo-type-jsonserializeroptions"></a>

## GetTypeInfo(Type, JsonSerializerOptions)

- Name: `GetTypeInfo(Type, JsonSerializerOptions)`
- Modifiers: `nullable`
- Returns: `JsonTypeInfo?`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.54628ef221bd1ad09c6ebc55da16ddcb5027f4c7/src/Foundation/JsonConverterTypeInfoResolver.cs#L50)

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
