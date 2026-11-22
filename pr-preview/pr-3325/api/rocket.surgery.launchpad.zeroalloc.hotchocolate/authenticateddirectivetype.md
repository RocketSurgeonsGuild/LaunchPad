# AuthenticatedDirectiveType

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate.md)
- Version: `2.0.1-pr.1294`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b12891a76083b718e1ed50b524f1c30a2c5447d7/src/ZeroAlloc.HotChocolate/Authorization/ScopeDirectives.cs)
- Inherits: `DirectiveType<AuthenticatedDirective>`

The type of `@authenticated`. It states what is required; the field's authorization enforces it.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.HotChocolate.Authorization;

public sealed class AuthenticatedDirectiveType
    : HotChocolate.Types.DirectiveType<Rocket.Surgery.LaunchPad.HotChocolate.Authorization.AuthenticatedDirective>
{
    // ...
}
```

## Constructors

- [AuthenticatedDirectiveType](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/authenticateddirectivetype/constructors.md#constructor)
