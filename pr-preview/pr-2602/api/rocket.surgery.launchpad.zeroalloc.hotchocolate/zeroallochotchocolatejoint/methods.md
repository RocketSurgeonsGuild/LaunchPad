# ZeroAllocHotChocolateJoint Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.zeroalloc.hotchocolate.md)
- Type: [ZeroAllocHotChocolateJoint](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/zeroallochotchocolatejoint.md)
- Kind: `Methods`
- Members: `1`

Reports each failure of a request the ZeroAlloc mediator rejected as a GraphQL `VALIDATION` error. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-2602-merge.Sha.f387c871fa01cb1c7bfc78f0117912dc30d931f2/src/ZeroAlloc.HotChocolate/Joints/ZeroAllocHotChocolateJoint.cs#L25-L27)

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
