# StrongTypeFormatAttribute

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1182`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
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

- [StrongTypeFormatAttribute(string)](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.primitives/strongtypeformatattribute/constructors.md#constructor-string) -- The format a strong type's value is written in, as OpenAPI and JSON Schema name it ( `email`, `uri`, `ipv4`, ...), on the strong type or on its rules.

## Properties

- [Format](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.primitives/strongtypeformatattribute/properties.md#format) : `string` `get` -- The format.
