# NodaTimeSystemTextJsonJoint Methods

- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.nodatime.md)
- Type: [NodaTimeSystemTextJsonJoint](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.nodatime/nodatimesystemtextjsonjoint.md)
- Kind: `Methods`
- Members: `1`

A joint that configures `Json.JsonSerializerOptions` with the NodaTime converters, using the `IDateTimeZoneProvider` that is registered. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3281-merge.Sha.f470c0fad9a519f7357ef1c1f00f2210962f5308/src/NodaTime/Joints/NodaTimeSystemTextJsonJoint.cs#L27-L37)

Register the Service part.

```csharp
public class NodaTimeSystemTextJsonJoint
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
