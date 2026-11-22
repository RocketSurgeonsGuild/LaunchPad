# IncludeInJsonContextAttribute

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3329/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1280`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3329-merge.Sha.a2bf3f292dae7cdf0854fdc32bf25e9839e7110d/src/Foundation/IncludeInJsonContextAttribute.cs)
- Inherits: `Attribute`

Puts a type in the JSON context LaunchPad generates in the applications that reference this assembly, for a type that crosses the wire as JSON but isn't a mediator request or response.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public sealed class IncludeInJsonContextAttribute
    : System.Attribute
{
    // ...
}
```

## Constructors

- [IncludeInJsonContextAttribute](/LaunchPad/pr-preview/pr-3329/api/rocket.surgery.launchpad.foundation/includeinjsoncontextattribute/constructors.md#constructor)
