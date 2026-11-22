# NodaTimeHotChocolateJoint Methods

- Package: [Rocket.Surgery.LaunchPad.NodaTime.HotChocolate](/LaunchPad/api/rocket.surgery.launchpad.nodatime.hotchocolate.md)
- Type: [NodaTimeHotChocolateJoint](/LaunchPad/api/rocket.surgery.launchpad.nodatime.hotchocolate/nodatimehotchocolatejoint.md)
- Kind: `Methods`
- Members: `1`

Adds the NodaTime types to the HotChocolate schema

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.974bfd9a73f64517e8a81d86b29393f3cce75778/src/NodaTime.HotChocolate/Joints/NodaTimeHotChocolateJoint.cs#L26-L32)

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
