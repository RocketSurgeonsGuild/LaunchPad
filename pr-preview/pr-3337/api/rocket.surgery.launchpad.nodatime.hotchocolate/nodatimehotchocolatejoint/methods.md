# NodaTimeHotChocolateJoint Methods

- Package: [Rocket.Surgery.LaunchPad.NodaTime.HotChocolate](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.nodatime.hotchocolate.md)
- Type: [NodaTimeHotChocolateJoint](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.nodatime.hotchocolate/nodatimehotchocolatejoint.md)
- Kind: `Methods`
- Members: `1`

Adds the NodaTime types to the HotChocolate schema

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3337-merge.Sha.b3468f8f7f271d2dc63ee1d3333c2bcdd48e93a2/src/NodaTime.HotChocolate/Joints/NodaTimeHotChocolateJoint.cs#L26-L32)

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
