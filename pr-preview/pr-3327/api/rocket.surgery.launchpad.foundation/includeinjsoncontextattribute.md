# IncludeInJsonContextAttribute

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3327/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1278`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3327-merge.Sha.f618e95e07fca6c7011f5e648f5b4966f567c6b1/src/Foundation/IncludeInJsonContextAttribute.cs)
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

- [IncludeInJsonContextAttribute](/LaunchPad/pr-preview/pr-3327/api/rocket.surgery.launchpad.foundation/includeinjsoncontextattribute/constructors.md#constructor)
