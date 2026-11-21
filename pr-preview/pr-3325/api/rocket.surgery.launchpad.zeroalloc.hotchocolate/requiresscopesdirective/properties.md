# RequiresScopesDirective Properties

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate.md)
- Type: [RequiresScopesDirective](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/requiresscopesdirective.md)
- Kind: `Properties`
- Members: `1`

The `@requiresScopes` directive of a field: the scopes a caller needs, as alternatives. The caller needs every scope of any one inner list. It has the name and shape of the composite schema directive, so gateways and tools that know that one read it.

<a id="scopes"></a>

## Scopes

- Name: `Scopes`
- Modifiers: `get`
- Returns: `IReadOnlyList<IReadOnlyList<string>>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.HotChocolate/Authorization/ScopeDirectives.cs#L17)

The alternatives: the outer list is OR, each inner list is AND.

```csharp
public IReadOnlyList<IReadOnlyList<string>> Scopes { get; }
```
