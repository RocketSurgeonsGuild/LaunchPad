# LaunchPadJsonSerializerOptionsExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3327/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1278`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3327-merge.Sha.f618e95e07fca6c7011f5e648f5b4966f567c6b1/src/Foundation/LaunchPadJsonSerializerOptionsExtensions.cs)

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

- [AddTypeInfoResolvers(JsonSerializerOptions, IEnumerable<JsonTypeInfoResolverRegistration>)](/LaunchPad/pr-preview/pr-3327/api/rocket.surgery.launchpad.foundation/launchpadjsonserializeroptionsextensions/methods.md#addtypeinforesolvers-jsonserializeroptions-ienumerable-jsontypeinforesolverregistration) : `JsonSerializerOptions` `extension` -- Puts the resolvers in front of the options' `JsonSerializerOptions.TypeInfoResolverChain`, ordered by [JsonTypeInfoResolverRegistration.Order](/LaunchPad/pr-preview/pr-3327/api/rocket.surgery.launchpad.foundation/jsontypeinforesolverregistration/properties.md#order).
