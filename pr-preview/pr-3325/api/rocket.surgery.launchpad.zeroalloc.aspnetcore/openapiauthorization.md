# OpenApiAuthorization

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Version: `2.0.1-pr.1304`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.AspNetCore/Authorization/OpenApiAuthorization.cs)

What the generated servers' OpenAPI transformers call to describe the authorization of an operation.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.Authorization;

public static class OpenApiAuthorization
{
    // ...
}
```

## Methods

- [RequireScopes(OpenApiOperation, OpenApiOperationTransformerContext)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/openapiauthorization/methods.md#requirescopes-openapioperation-openapioperationtransformercontext) `static` -- States the scopes an operation requires, from the plan of the request it dispatches: a security requirement per alternative, any one of which is enough (docs/design/authorization.md, section 5). A request without a plan adds nothing.
