# StrongTypeFormatAttribute

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.434`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.dfbe8776933590a8ca1efba66f9af5c4929d4975/src/Primitives/Descriptions/StrongTypeDescription.cs)
- Inherits: `Attribute`

The format a strong type's value is written in, as OpenAPI and JSON Schema name it ( `email`, `uri`, `ipv4`, ...), on the strong type or on its rules.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class StrongTypeFormatAttribute
    : System.Attribute
{
    // ...
}
```

## Constructors

- [StrongTypeFormatAttribute(string)](/LaunchPad/api/rocket.surgery.launchpad.primitives/strongtypeformatattribute/constructors.md#constructor-string) -- The format a strong type's value is written in, as OpenAPI and JSON Schema name it ( `email`, `uri`, `ipv4`, ...), on the strong type or on its rules.

## Properties

- [Format](/LaunchPad/api/rocket.surgery.launchpad.primitives/strongtypeformatattribute/properties.md#format) : `string` `get` -- The format.
