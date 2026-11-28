# HotChocolateJoint Methods

- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.hotchocolate.md)
- Type: [HotChocolateJoint](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.hotchocolate/hotchocolatejoint.md)
- Kind: `Methods`
- Members: `1`

Hot Chocolate Joint

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.52b728c7d35f7e5541d5238c567b2b39c033d3c7/src/HotChocolate/Joints/HotChocolateJoint.cs#L21-L26)

Register the Service part.

```csharp
public class HotChocolateJoint
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
