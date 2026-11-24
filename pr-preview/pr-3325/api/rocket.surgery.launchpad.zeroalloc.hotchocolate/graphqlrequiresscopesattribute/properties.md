# GraphQLRequiresScopesAttribute Properties

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate.md)
- Type: [GraphQLRequiresScopesAttribute](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/graphqlrequiresscopesattribute.md)
- Kind: `Properties`
- Members: `1`

States the scopes a field requires with `@requiresScopes`. The generated GraphQL hosts put it on the field of an operation whose request has policies that declare scopes.

<a id="alternatives"></a>

## Alternatives

- Name: `Alternatives`
- Modifiers: `get`
- Returns: `string[]`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.HotChocolate/Authorization/ScopeDirectives.cs#L65)

The alternatives, each the scopes of one way in, separated by spaces.

```csharp
public string[] Alternatives { get; }
```
