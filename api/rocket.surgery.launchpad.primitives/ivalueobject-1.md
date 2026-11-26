# IValueObject<TSelf>

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.456`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.0c205f7c0f7c99d017c3755b484180de4bcb850a/src/Primitives/IValueObject.cs)
- Implements: [IDomainPrimitive<TSelf>](/LaunchPad/api/rocket.surgery.launchpad.primitives/idomainprimitive-1.md)

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

- [Description](/LaunchPad/api/rocket.surgery.launchpad.primitives/ivalueobject-1/properties.md#description) : [ValueObjectDescription](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectdescription.md) `static` `abstract` `get` -- What an integration needs to know about the value object: the values it holds.
