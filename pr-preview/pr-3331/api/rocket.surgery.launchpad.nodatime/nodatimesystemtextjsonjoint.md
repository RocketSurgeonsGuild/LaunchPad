# NodaTimeSystemTextJsonJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.nodatime.md)
- Version: `2.0.1-pr.1284`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3331-merge.Sha.4e9f7c28a9b18bc67da869f4b85f2484ef7df001/src/NodaTime/Joints/NodaTimeSystemTextJsonJoint.cs)
- Implements: `IDovetailJoint`, `IServiceJoint`

A joint that configures `Json.JsonSerializerOptions` with the NodaTime converters, using the `IDateTimeZoneProvider` that is registered. Implements the `Joints.IServiceJoint`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.NodaTime.Joints;

public class NodaTimeSystemTextJsonJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Remarks

The time zone provider is resolved when the options are first built, so it does not matter whether it is registered before or after this joint. The [TimeJoint](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.nodatime/timejoint.md) registers the provider an application uses by default.

## Constructors

- [NodaTimeSystemTextJsonJoint](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.nodatime/nodatimesystemtextjsonjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.nodatime/nodatimesystemtextjsonjoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.

## See Also

- `Joints.IServiceJoint`
