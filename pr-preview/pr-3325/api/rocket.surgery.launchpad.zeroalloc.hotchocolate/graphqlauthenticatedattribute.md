# GraphQLAuthenticatedAttribute

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate.md)
- Version: `2.0.1-pr.1286`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.HotChocolate/Authorization/ScopeDirectives.cs)
- Inherits: `ObjectFieldDescriptorAttribute`

States that a field requires an authenticated caller with `@authenticated`. The generated GraphQL hosts put it on the field of an operation whose request has policies that declare no scopes.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.HotChocolate.Authorization;

public sealed class GraphQLAuthenticatedAttribute
    : HotChocolate.Types.ObjectFieldDescriptorAttribute
{
    // ...
}
```

## Constructors

- [GraphQLAuthenticatedAttribute](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/graphqlauthenticatedattribute/constructors.md#constructor)
