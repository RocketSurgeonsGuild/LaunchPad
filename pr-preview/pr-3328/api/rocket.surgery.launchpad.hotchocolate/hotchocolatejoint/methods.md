# HotChocolateJoint Methods

- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3328/api/rocket.surgery.launchpad.hotchocolate.md)
- Type: [HotChocolateJoint](/LaunchPad/pr-preview/pr-3328/api/rocket.surgery.launchpad.hotchocolate/hotchocolatejoint.md)
- Kind: `Methods`
- Members: `1`

Hot Chocolate Joint

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3328-merge.Sha.e3c937ccb9d716ab226fa523eee028ac17c7ed68/src/HotChocolate/Joints/HotChocolateJoint.cs#L21-L26)

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
