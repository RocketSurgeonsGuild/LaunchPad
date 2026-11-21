# LaunchPadJsonSerializerOptionsExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1275`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3323-merge.Sha.848d7df41fb82a23ce0738e64978a35c934d7558/src/Foundation/LaunchPadJsonSerializerOptionsExtensions.cs)

Builds the `JsonSerializerOptions.TypeInfoResolverChain` of a `Json.JsonSerializerOptions` from registered resolvers.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public static class LaunchPadJsonSerializerOptionsExtensions
{
    // ...
}
```

## Methods

- [AddTypeInfoResolvers(JsonSerializerOptions, IEnumerable<JsonTypeInfoResolverRegistration>)](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.foundation/launchpadjsonserializeroptionsextensions/methods.md#addtypeinforesolvers-jsonserializeroptions-ienumerable-jsontypeinforesolverregistration) : `JsonSerializerOptions` `extension` -- Puts the resolvers in front of the options' `JsonSerializerOptions.TypeInfoResolverChain`, ordered by [JsonTypeInfoResolverRegistration.Order](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.foundation/jsontypeinforesolverregistration/properties.md#order).
