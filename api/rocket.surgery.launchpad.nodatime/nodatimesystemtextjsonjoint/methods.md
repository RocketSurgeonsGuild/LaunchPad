# NodaTimeSystemTextJsonJoint Methods

- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/api/rocket.surgery.launchpad.nodatime.md)
- Type: [NodaTimeSystemTextJsonJoint](/LaunchPad/api/rocket.surgery.launchpad.nodatime/nodatimesystemtextjsonjoint.md)
- Kind: `Methods`
- Members: `1`

A joint that configures `Json.JsonSerializerOptions` with the NodaTime converters, using the `IDateTimeZoneProvider` that is registered. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.5756d1abb55aa6348e0f70f98329f405a8d0c1f6/src/NodaTime/Joints/NodaTimeSystemTextJsonJoint.cs#L27-L37)

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
