# RequiresScopesDirective Constructors

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate.md)
- Type: [RequiresScopesDirective](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/requiresscopesdirective.md)
- Kind: `Constructors`
- Members: `1`

The `@requiresScopes` directive of a field: the scopes a caller needs, as alternatives. The caller needs every scope of any one inner list. It has the name and shape of the composite schema directive, so gateways and tools that know that one read it.

<a id="constructor"></a>
<a id="constructor-ireadonlylist-ireadonlylist-string"></a>

## RequiresScopesDirective(IReadOnlyList<IReadOnlyList<string>>)

- Name: `Constructor(IReadOnlyList<IReadOnlyList<string>>)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.HotChocolate/Authorization/ScopeDirectives.cs#L14-L17)

The `@requiresScopes` directive of a field: the scopes a caller needs, as alternatives. The caller needs every scope of any one inner list. It has the name and shape of the composite schema directive, so gateways and tools that know that one read it.

```csharp
public sealed class RequiresScopesDirective
{
    public RequiresScopesDirective(
        IReadOnlyList<IReadOnlyList<string>> scopes)
    {
        // ...
    }
}
```

## Parameters

- `scopes` (`IReadOnlyList<IReadOnlyList<string>>`)
  The alternatives: the outer list is OR, each inner list is AND.
