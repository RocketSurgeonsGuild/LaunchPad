# StrongTypeFormatAttribute

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1284`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3332-merge.Sha.108cb13ce492322587639698c87355d3b8fcee1b/src/Primitives/Descriptions/StrongTypeDescription.cs)
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

- [StrongTypeFormatAttribute(string)](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives/strongtypeformatattribute/constructors.md#constructor-string) -- The format a strong type's value is written in, as OpenAPI and JSON Schema name it ( `email`, `uri`, `ipv4`, ...), on the strong type or on its rules.

## Properties

- [Format](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives/strongtypeformatattribute/properties.md#format) : `string` `get` -- The format.
