# TimeJoint Methods

- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-3341/api/rocket.surgery.launchpad.nodatime.md)
- Type: [TimeJoint](/LaunchPad/pr-preview/pr-3341/api/rocket.surgery.launchpad.nodatime/timejoint.md)
- Kind: `Methods`
- Members: `1`

A joint that registers the clock, time provider and time zone provider. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3341-merge.Sha.69b2d11eb32071e6ab9f05439eb8c02f3ae46ce6/src/NodaTime/Joints/TimeJoint.cs#L24-L31)

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
