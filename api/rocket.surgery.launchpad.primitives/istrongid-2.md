# IStrongId<TSelf, TValue>

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.462`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.cd7db14e49b12478b13b2f21edf214c613078b4c/src/Primitives/IStrongId.cs)
- Implements: [IDomainPrimitive<TSelf, TValue>](/LaunchPad/api/rocket.surgery.launchpad.primitives/idomainprimitive-2.md), [IDomainPrimitive<TSelf>](/LaunchPad/api/rocket.surgery.launchpad.primitives/idomainprimitive-1.md)

A strong ID: a domain primitive that identifies an entity.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives;

public interface IStrongId<TSelf, TValue>
    : Rocket.Surgery.LaunchPad.Primitives.IDomainPrimitive<TSelf, TValue>,
      Rocket.Surgery.LaunchPad.Primitives.IDomainPrimitive<TSelf>
    where TSelf : IStrongId<TSelf, TValue>
    where TValue : notnull
{
    // ...
}
```

## Type Parameters

- `TSelf` -- The strong ID.
- `TValue` -- The value it holds: a `Guid`, `Int32`, `Int64` or `String`.

## Properties

- [Empty](/LaunchPad/api/rocket.surgery.launchpad.primitives/istrongid-2/properties.md#empty) : `TSelf` `static` `abstract` `get` -- The empty ID.

## Methods

- [Create](/LaunchPad/api/rocket.surgery.launchpad.primitives/istrongid-2/methods.md#create) : `TSelf` `static` `abstract` -- Creates a new ID. Only an ID of a `Guid` can be created; any other throws `NotSupportedException`.
