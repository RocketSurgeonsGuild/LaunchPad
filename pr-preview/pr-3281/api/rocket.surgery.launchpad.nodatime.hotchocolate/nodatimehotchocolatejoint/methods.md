# NodaTimeHotChocolateJoint Methods

- Package: [Rocket.Surgery.LaunchPad.NodaTime.HotChocolate](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.nodatime.hotchocolate.md)
- Type: [NodaTimeHotChocolateJoint](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.nodatime.hotchocolate/nodatimehotchocolatejoint.md)
- Kind: `Methods`
- Members: `1`

Adds the NodaTime types to the HotChocolate schema

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3281-merge.Sha.3325c774c1aaaf8fd0e7d3543b322e84255d322c/src/NodaTime.HotChocolate/Joints/NodaTimeHotChocolateJoint.cs#L26-L32)

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
