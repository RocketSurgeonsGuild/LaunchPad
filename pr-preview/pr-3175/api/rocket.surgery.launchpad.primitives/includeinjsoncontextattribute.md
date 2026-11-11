# IncludeInJsonContextAttribute

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1167`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net10.0`
- Inherits: `Attribute`

Puts a type in the JSON context LaunchPad generates in the applications that reference this assembly, for a type that crosses the wire as JSON but isn't a mediator request or response.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives;

public sealed class IncludeInJsonContextAttribute
    : System.Attribute
{
    // ...
}
```

## Constructors

- [IncludeInJsonContextAttribute](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives/includeinjsoncontextattribute/constructors.md#constructor)
