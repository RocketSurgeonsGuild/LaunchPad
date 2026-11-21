# GraphQLRequiresScopesAttribute Constructors

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate.md)
- Type: [GraphQLRequiresScopesAttribute](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/graphqlrequiresscopesattribute.md)
- Kind: `Constructors`
- Members: `1`

States the scopes a field requires with `@requiresScopes`. The generated GraphQL hosts put it on the field of an operation whose request has policies that declare scopes.

<a id="constructor"></a>
<a id="constructor-string"></a>

## GraphQLRequiresScopesAttribute(string[])

- Name: `Constructor(string[])`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.HotChocolate/Authorization/ScopeDirectives.cs#L62-L65)

States the scopes a field requires with `@requiresScopes`. The generated GraphQL hosts put it on the field of an operation whose request has policies that declare scopes.

```csharp
public sealed class GraphQLRequiresScopesAttribute
{
    public GraphQLRequiresScopesAttribute(
        params string[] alternatives)
    {
        // ...
    }
}
```

## Parameters

- `alternatives` (`string[]`)
  The alternatives, each the scopes of one way in, separated by spaces.
