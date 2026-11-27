# IValueObject<TSelf>

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1300`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3347-merge.Sha.c2b6537b4ab49f853c50fea9f1114836c5d8adf2/src/Primitives/IValueObject.cs)
- Implements: [IDomainPrimitive<TSelf>](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.primitives/idomainprimitive-1.md)

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

- [Description](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.primitives/ivalueobject-1/properties.md#description) : [ValueObjectDescription](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.primitives/valueobjectdescription.md) `static` `abstract` `get` -- What an integration needs to know about the value object: the values it holds.
