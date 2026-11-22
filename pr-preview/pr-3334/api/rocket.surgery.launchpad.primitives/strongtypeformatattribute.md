# StrongTypeFormatAttribute

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3334/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1284`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3334-merge.Sha.1bdca6582f82d4de4d3840709c4a6fa63d4c5f0c/src/Primitives/Descriptions/StrongTypeDescription.cs)
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

- [StrongTypeFormatAttribute(string)](/LaunchPad/pr-preview/pr-3334/api/rocket.surgery.launchpad.primitives/strongtypeformatattribute/constructors.md#constructor-string) -- The format a strong type's value is written in, as OpenAPI and JSON Schema name it ( `email`, `uri`, `ipv4`, ...), on the strong type or on its rules.

## Properties

- [Format](/LaunchPad/pr-preview/pr-3334/api/rocket.surgery.launchpad.primitives/strongtypeformatattribute/properties.md#format) : `string` `get` -- The format.
