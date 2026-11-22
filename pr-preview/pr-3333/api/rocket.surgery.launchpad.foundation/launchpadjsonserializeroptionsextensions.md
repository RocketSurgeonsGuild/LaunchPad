# LaunchPadJsonSerializerOptionsExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3333/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1284`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3333-merge.Sha.836b758fb89040b6cc85ed7817191956e959bc1f/src/Foundation/LaunchPadJsonSerializerOptionsExtensions.cs)

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

- [AddTypeInfoResolvers(JsonSerializerOptions, IEnumerable<JsonTypeInfoResolverRegistration>)](/LaunchPad/pr-preview/pr-3333/api/rocket.surgery.launchpad.foundation/launchpadjsonserializeroptionsextensions/methods.md#addtypeinforesolvers-jsonserializeroptions-ienumerable-jsontypeinforesolverregistration) : `JsonSerializerOptions` `extension` -- Puts the resolvers in front of the options' `JsonSerializerOptions.TypeInfoResolverChain`, ordered by [JsonTypeInfoResolverRegistration.Order](/LaunchPad/pr-preview/pr-3333/api/rocket.surgery.launchpad.foundation/jsontypeinforesolverregistration/properties.md#order).
