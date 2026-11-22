# GraphQLRequiresScopesAttribute

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate.md)
- Version: `2.0.1-pr.1291`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.HotChocolate/Authorization/ScopeDirectives.cs)
- Inherits: `ObjectFieldDescriptorAttribute`

States the scopes a field requires with `@requiresScopes`. The generated GraphQL hosts put it on the field of an operation whose request has policies that declare scopes.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.HotChocolate.Authorization;

public sealed class GraphQLRequiresScopesAttribute
    : HotChocolate.Types.ObjectFieldDescriptorAttribute
{
    // ...
}
```

## Constructors

- [GraphQLRequiresScopesAttribute(string[])](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/graphqlrequiresscopesattribute/constructors.md#constructor-string) -- States the scopes a field requires with `@requiresScopes`. The generated GraphQL hosts put it on the field of an operation whose request has policies that declare scopes.

## Properties

- [Alternatives](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/graphqlrequiresscopesattribute/properties.md#alternatives) : `string[]` `get` -- The alternatives, each the scopes of one way in, separated by spaces.
