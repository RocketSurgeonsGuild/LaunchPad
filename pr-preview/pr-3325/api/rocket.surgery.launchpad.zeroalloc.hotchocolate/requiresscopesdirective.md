# RequiresScopesDirective

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate.md)
- Version: `2.0.1-pr.1291`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.HotChocolate/Authorization/ScopeDirectives.cs)

The `@requiresScopes` directive of a field: the scopes a caller needs, as alternatives. The caller needs every scope of any one inner list. It has the name and shape of the composite schema directive, so gateways and tools that know that one read it.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.HotChocolate.Authorization;

public sealed class RequiresScopesDirective
{
    // ...
}
```

## Constructors

- [RequiresScopesDirective(IReadOnlyList<IReadOnlyList<string>>)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/requiresscopesdirective/constructors.md#constructor-ireadonlylist-ireadonlylist-string) -- The `@requiresScopes` directive of a field: the scopes a caller needs, as alternatives. The caller needs every scope of any one inner list. It has the name and shape of the composite schema directive, so gateways and tools that know that one read it.

## Properties

- [Scopes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/requiresscopesdirective/properties.md#scopes) : `IReadOnlyList<IReadOnlyList<string>>` `get` -- The alternatives: the outer list is OR, each inner list is AND.
