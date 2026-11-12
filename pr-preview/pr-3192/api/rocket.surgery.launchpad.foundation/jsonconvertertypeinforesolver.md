# JsonConverterTypeInfoResolver

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1180`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net10.0`
- Implements: `IJsonTypeInfoResolver`

Resolves the metadata of value types written by a converter in `JsonSerializerOptions.Converters`, such as the NodaTime or the spatial types, without reflection.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public sealed class JsonConverterTypeInfoResolver
    : System.Text.Json.Serialization.Metadata.IJsonTypeInfoResolver
{
    // ...
}
```

## Remarks

A source generated `Serialization.JsonSerializerContext` only knows the types it declares. When such a type is serialized as an `Object` (a problem details extension, a health check value) or at the root, the options still need metadata for it; this resolver builds it from the converter the options already have. A type with no converter in the options is left to the next resolver in the chain.

## Constructors

- [JsonConverterTypeInfoResolver](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.foundation/jsonconvertertypeinforesolver/constructors.md#constructor)

## Properties

- [Types](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.foundation/jsonconvertertypeinforesolver/properties.md#types) : `IEnumerable<Type>` `get` -- The types this resolver knows.

## Methods

- [Add](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.foundation/jsonconvertertypeinforesolver/methods.md#add) : [JsonConverterTypeInfoResolver](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.foundation/jsonconvertertypeinforesolver.md) -- Resolves `T` with the converter for it in the options.
- [AddNullable](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.foundation/jsonconvertertypeinforesolver/methods.md#addnullable) : [JsonConverterTypeInfoResolver](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.foundation/jsonconvertertypeinforesolver.md) -- Resolves `T` and `Nullable`1` with the converter for `T` in the options.
- [GetTypeInfo(Type, JsonSerializerOptions)](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.foundation/jsonconvertertypeinforesolver/methods.md#gettypeinfo-type-jsonserializeroptions) : `JsonTypeInfo?` -- Resolves a `Metadata.JsonTypeInfo` contract for the requested type and options.
