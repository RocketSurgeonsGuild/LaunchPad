# IValueObject<TSelf>

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3349/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1293`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3349-merge.Sha.70168524ff777a6c4fd1a4bf57611d64095a2c00/src/Primitives/IValueObject.cs)
- Implements: [IDomainPrimitive<TSelf>](/LaunchPad/pr-preview/pr-3349/api/rocket.surgery.launchpad.primitives/idomainprimitive-1.md)

A value object: a domain primitive that holds several values, validated together.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives;

public interface IValueObject<TSelf>
    : Rocket.Surgery.LaunchPad.Primitives.IDomainPrimitive<TSelf>
    where TSelf : IValueObject<TSelf>
{
    // ...
}
```

## Type Parameters

- `TSelf` -- The value object.

## Properties

- [Description](/LaunchPad/pr-preview/pr-3349/api/rocket.surgery.launchpad.primitives/ivalueobject-1/properties.md#description) : [ValueObjectDescription](/LaunchPad/pr-preview/pr-3349/api/rocket.surgery.launchpad.primitives/valueobjectdescription.md) `static` `abstract` `get` -- What an integration needs to know about the value object: the values it holds.
