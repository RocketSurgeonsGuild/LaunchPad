# ZeroAllocHotChocolateJoint Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate](/LaunchPad/pr-preview/pr-3308/api/rocket.surgery.launchpad.zeroalloc.hotchocolate.md)
- Type: [ZeroAllocHotChocolateJoint](/LaunchPad/pr-preview/pr-3308/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/zeroallochotchocolatejoint.md)
- Kind: `Methods`
- Members: `1`

Reports each failure of a request the ZeroAlloc mediator rejected as a GraphQL `VALIDATION` error. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3308-merge.Sha.b5321c7138bef72baa9f203b3f824fa7207c9dcf/src/ZeroAlloc.HotChocolate/Joints/ZeroAllocHotChocolateJoint.cs#L25-L27)

Register the Service part.

```csharp
public class ZeroAllocHotChocolateJoint
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
