# TimeJoint Methods

- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.nodatime.md)
- Type: [TimeJoint](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.nodatime/timejoint.md)
- Kind: `Methods`
- Members: `1`

A joint that registers the clock, time provider and time zone provider. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3320-merge.Sha.bb93b2eb5e069135a2caef17855a85e8a18d22af/src/NodaTime/Joints/TimeJoint.cs#L24-L31)

Register the Service part.

```csharp
public class TimeJoint
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
