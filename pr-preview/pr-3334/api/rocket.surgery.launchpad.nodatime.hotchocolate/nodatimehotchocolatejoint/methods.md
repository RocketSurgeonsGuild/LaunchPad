# NodaTimeHotChocolateJoint Methods

- Package: [Rocket.Surgery.LaunchPad.NodaTime.HotChocolate](/LaunchPad/pr-preview/pr-3334/api/rocket.surgery.launchpad.nodatime.hotchocolate.md)
- Type: [NodaTimeHotChocolateJoint](/LaunchPad/pr-preview/pr-3334/api/rocket.surgery.launchpad.nodatime.hotchocolate/nodatimehotchocolatejoint.md)
- Kind: `Methods`
- Members: `1`

Adds the NodaTime types to the HotChocolate schema

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3334-merge.Sha.1bdca6582f82d4de4d3840709c4a6fa63d4c5f0c/src/NodaTime.HotChocolate/Joints/NodaTimeHotChocolateJoint.cs#L26-L32)

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
