# NodaTimeStrawberryShakeJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake](/LaunchPad/pr-preview/pr-3308/api/rocket.surgery.launchpad.nodatime.strawberryshake.md)
- Version: `2.0.1-pr.1261`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3308-merge.Sha.b5321c7138bef72baa9f203b3f824fa7207c9dcf/src/NodaTime.StrawberryShake/Joints/NodaTimeStrawberryShakeJoint.cs)
- Implements: `IDovetailJoint`, `IServiceJoint`

Adds the NodaTime serializers to StrawberryShake

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake.Joints;

public class NodaTimeStrawberryShakeJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Remarks

Every NodaTime scalar that HotChocolate 16's `AddNodaTime` puts in a schema has a serializer here, and all of them are registered: `DateTime` ( `OffsetDateTime`), `Duration`, `LocalDate`, `LocalDateTime` and `LocalTime`. A generated client resolves a serializer for every scalar its operations use, and cannot create its serializer resolver, for any operation, when one of them is missing. A client maps the scalars to the NodaTime types with `@runtimeType` in its `schema.extensions.graphql`.

## Constructors

- [NodaTimeStrawberryShakeJoint](/LaunchPad/pr-preview/pr-3308/api/rocket.surgery.launchpad.nodatime.strawberryshake/nodatimestrawberryshakejoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3308/api/rocket.surgery.launchpad.nodatime.strawberryshake/nodatimestrawberryshakejoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.
