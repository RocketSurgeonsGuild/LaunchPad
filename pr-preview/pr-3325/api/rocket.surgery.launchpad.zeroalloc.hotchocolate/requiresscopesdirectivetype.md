# RequiresScopesDirectiveType

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate.md)
- Version: `2.0.1-pr.1298`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.1b696446ea1fb50dee983bcac42661335aad175b/src/ZeroAlloc.HotChocolate/Authorization/ScopeDirectives.cs)
- Inherits: `DirectiveType<RequiresScopesDirective>`

The type of `@requiresScopes(scopes: [[String!]!]!)`. It states what is required; the field's authorization enforces it.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.HotChocolate.Authorization;

public sealed class RequiresScopesDirectiveType
    : HotChocolate.Types.DirectiveType<Rocket.Surgery.LaunchPad.HotChocolate.Authorization.RequiresScopesDirective>
{
    // ...
}
```

## Constructors

- [RequiresScopesDirectiveType](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/requiresscopesdirectivetype/constructors.md#constructor)
