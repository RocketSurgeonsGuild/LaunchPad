# ZeroAllocAuthorizationJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Version: `2.0.1-pr.1304`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.AspNetCore/Joints/ZeroAllocAuthorizationJoint.cs)
- Implements: `IDovetailJoint`, `IServiceJoint`

Checks the policies of a request at the edge of an ASP.NET Core application, before it is bound, and describes them in the OpenAPI document (docs/design/authorization.md, sections 4 and 5). Implements the `Joints.IServiceJoint`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

public class ZeroAllocAuthorizationJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Remarks

The caller policies see is the one ASP.NET Core authenticated, unless the application registered its own `Authorization.ISecurityContext`. The generated servers name the policies of a request on its operation; each is answered with an ASP.NET Core policy that runs it and records the outcome the mediator pipeline then reuses. What it rejects is answered as the `Foundation.Unauthorized` or `Foundation.Forbidden` case, with the scopes in `WWW-Authenticate`.

An application that registers its own `Authorization.IAuthorizationPolicyProvider` or `Authorization.IAuthorizationMiddlewareResultHandler` after this joint replaces LaunchPad's, and with it the edge.

## Constructors

- [ZeroAllocAuthorizationJoint](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocauthorizationjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocauthorizationjoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.

## See Also

- `Joints.IServiceJoint`
