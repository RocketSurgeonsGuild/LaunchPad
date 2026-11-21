# NodaTimeHotChocolateJoint Methods

- Package: [Rocket.Surgery.LaunchPad.NodaTime.HotChocolate](/LaunchPad/pr-preview/pr-3319/api/rocket.surgery.launchpad.nodatime.hotchocolate.md)
- Type: [NodaTimeHotChocolateJoint](/LaunchPad/pr-preview/pr-3319/api/rocket.surgery.launchpad.nodatime.hotchocolate/nodatimehotchocolatejoint.md)
- Kind: `Methods`
- Members: `1`

Adds the NodaTime types to the HotChocolate schema

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3319-merge.Sha.db5ef919c4f6a03a7f33e4672dc70757d169d5a4/src/NodaTime.HotChocolate/Joints/NodaTimeHotChocolateJoint.cs#L26-L32)

Register the Service part.

```csharp
public class NodaTimeHotChocolateJoint
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
