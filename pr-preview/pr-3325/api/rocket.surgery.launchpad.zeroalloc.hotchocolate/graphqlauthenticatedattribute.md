# GraphQLAuthenticatedAttribute

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate.md)
- Version: `2.0.1-pr.1304`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.HotChocolate/Authorization/ScopeDirectives.cs)
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
