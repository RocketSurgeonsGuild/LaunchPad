# IStrongId<TSelf, TValue>

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1290`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3345-merge.Sha.a9c0325e0c8c70bc9d0569ac4df1fe5d29ffa4ec/src/Primitives/IStrongId.cs)
- Implements: [IDomainPrimitive<TSelf, TValue>](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.primitives/idomainprimitive-2.md), [IDomainPrimitive<TSelf>](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.primitives/idomainprimitive-1.md)

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

- [Empty](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.primitives/istrongid-2/properties.md#empty) : `TSelf` `static` `abstract` `get` -- The empty ID.

## Methods

- [Create](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.primitives/istrongid-2/methods.md#create) : `TSelf` `static` `abstract` -- Creates a new ID. Only an ID of a `Guid` can be created; any other throws `NotSupportedException`.
