# ZeroAllocAuthorizationJoint Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Type: [ZeroAllocAuthorizationJoint](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/zeroallocauthorizationjoint.md)
- Kind: `Methods`
- Members: `1`

Checks the policies of a request at the edge of an ASP.NET Core application, before it is bound, and describes them in the OpenAPI document (docs/design/authorization.md, sections 4 and 5). Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.AspNetCore/Joints/ZeroAllocAuthorizationJoint.cs#L43-L72)

Register the Service part.

```csharp
public class ZeroAllocAuthorizationJoint
{
    public void Register(
        IDovetailContext context,
        IServiceCollection services)
    {
        // ...
    }
}
```

## Parameters

- `context` (`IDovetailContext`)
- `services` (`IServiceCollection`)
