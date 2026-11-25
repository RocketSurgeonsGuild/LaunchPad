# LaunchPadJsonEnumConverters

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1290`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.7cda342c7a30094b955266549c4928a5c52b02bb/src/Foundation/LaunchPadJsonEnumConverters.cs)

The enum converters the `SystemTextJsonJoint` adds to the `Json.JsonSerializerOptions` it configures.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public static class LaunchPadJsonEnumConverters
{
    // ...
}
```

## Remarks

The generator that ships with this package registers a camel case `Serialization.JsonStringEnumConverter`1` here, from a module initializer, for every enum a JSON context of the assembly reaches. Generic converters need no dynamic code, so enums are written as camel case strings in a native AOT application as well; an enum no JSON context reaches is written as a number.

## Properties

- [Converters](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.foundation/launchpadjsonenumconverters/properties.md#converters) : `IReadOnlyCollection<JsonConverter>` `static` `get` -- The converters registered so far.

## Methods

- [AddEnumConverters(JsonSerializerOptions)](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.foundation/launchpadjsonenumconverters/methods.md#addenumconverters-jsonserializeroptions) : `JsonSerializerOptions` `extension` -- Adds every registered converter the options don't have yet.
- [Register(JsonStringEnumConverter<TEnum>)](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.foundation/launchpadjsonenumconverters/methods.md#register-jsonstringenumconverter-tenum) `static` -- Registers an enum converter. A later registration for the same type replaces the earlier one.
