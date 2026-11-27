# IDomainPrimitive<TSelf, TValue>

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.458`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.6a854476dc50a3b95bf56cf9e7b0d45ee9f0bfa7/src/Primitives/IDomainPrimitive.cs)
- Implements: [IDomainPrimitive<TSelf>](/LaunchPad/api/rocket.surgery.launchpad.primitives/idomainprimitive-1.md)

A domain primitive that holds one value.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives;

public interface IDomainPrimitive<TSelf, TValue>
    : Rocket.Surgery.LaunchPad.Primitives.IDomainPrimitive<TSelf>
    where TSelf : IDomainPrimitive<TSelf, TValue>
    where TValue : notnull
{
    // ...
}
```

## Type Parameters

- `TSelf` -- The domain primitive.
- `TValue` -- The value it holds.

## Properties

- [Description](/LaunchPad/api/rocket.surgery.launchpad.primitives/idomainprimitive-2/properties.md#description) : `DomainPrimitiveDescription<TSelf, TValue>` `static` `abstract` `get` -- What an integration needs to know about the domain primitive: its value and the constraints of its rules.
- [Value](/LaunchPad/api/rocket.surgery.launchpad.primitives/idomainprimitive-2/properties.md#value) : `TValue` `abstract` `get` -- The value.

## Methods

- [Create(TValue)](/LaunchPad/api/rocket.surgery.launchpad.primitives/idomainprimitive-2/methods.md#create-tvalue) : `TSelf` `static` `abstract` -- Creates the domain primitive from a value, or throws when its rules reject the value.
- [TryCreate(TValue, TSelf?, PrimitiveError)](/LaunchPad/api/rocket.surgery.launchpad.primitives/idomainprimitive-2/methods.md#trycreate-tvalue-tself-primitiveerror) : `bool` `static` `abstract` -- Creates the domain primitive from a value, or says why its rules reject the value.
