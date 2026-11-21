# LaunchPadJsonSerializerOptionsExtensions Methods

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3312/api/rocket.surgery.launchpad.foundation.md)
- Type: [LaunchPadJsonSerializerOptionsExtensions](/LaunchPad/pr-preview/pr-3312/api/rocket.surgery.launchpad.foundation/launchpadjsonserializeroptionsextensions.md)
- Kind: `Methods`
- Members: `1`

Builds the `JsonSerializerOptions.TypeInfoResolverChain` of a `Json.JsonSerializerOptions` from registered resolvers.

<a id="addtypeinforesolvers"></a>
<a id="addtypeinforesolvers-jsonserializeroptions-ienumerable-jsontypeinforesolverregistration"></a>

## AddTypeInfoResolvers(JsonSerializerOptions, IEnumerable<JsonTypeInfoResolverRegistration>)

- Name: `AddTypeInfoResolvers(JsonSerializerOptions, IEnumerable<JsonTypeInfoResolverRegistration>)`
- Modifiers: `extension`
- Returns: `JsonSerializerOptions`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3312-merge.Sha.f73ef9701b863c2f6314f4b9416fd99efc8c6135/src/Foundation/LaunchPadJsonSerializerOptionsExtensions.cs#L25-L42)

Puts the resolvers in front of the options' `JsonSerializerOptions.TypeInfoResolverChain`, ordered by [JsonTypeInfoResolverRegistration.Order](/LaunchPad/pr-preview/pr-3312/api/rocket.surgery.launchpad.foundation/jsontypeinforesolverregistration/properties.md#order).

```csharp
public static class LaunchPadJsonSerializerOptionsExtensions
{
    public static JsonSerializerOptions AddTypeInfoResolvers(
        this JsonSerializerOptions options,
        IEnumerable<JsonTypeInfoResolverRegistration> registrations)
    {
        // ...
    }
}
```

## Parameters

- `options` (`JsonSerializerOptions`)
  The options to configure.
- `registrations` ([IEnumerable<JsonTypeInfoResolverRegistration>](/LaunchPad/pr-preview/pr-3312/api/rocket.surgery.launchpad.foundation/jsontypeinforesolverregistration.md))
  The registered resolvers.

## Returns

`JsonSerializerOptions` -- The options.

## Remarks

LaunchPad never adds the reflection based resolver: a type no registered JSON context knows fails the same way in a JIT application as in a native AOT one. A reflection resolver the application put in the chain itself stays where it is. Resolvers already in the chain are not added again.
