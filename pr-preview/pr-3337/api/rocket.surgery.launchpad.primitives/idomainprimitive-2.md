# IDomainPrimitive<TSelf, TValue>

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1324`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3337-merge.Sha.09dd538c83fe5dc2188f0956cd1425be399f85c1/src/Primitives/IDomainPrimitive.cs)
- Implements: [IDomainPrimitive<TSelf>](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/idomainprimitive-1.md)

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

- [Description](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/idomainprimitive-2/properties.md#description) : `DomainPrimitiveDescription<TSelf, TValue>` `static` `abstract` `get` -- What an integration needs to know about the domain primitive: its value and the constraints of its rules.
- [Value](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/idomainprimitive-2/properties.md#value) : `TValue` `abstract` `get` -- The value.

## Methods

- [Create(TValue)](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/idomainprimitive-2/methods.md#create-tvalue) : `TSelf` `static` `abstract` -- Creates the domain primitive from a value, or throws when its rules reject the value.
- [TryCreate(TValue, TSelf?, PrimitiveError)](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/idomainprimitive-2/methods.md#trycreate-tvalue-tself-primitiveerror) : `bool` `static` `abstract` -- Creates the domain primitive from a value, or says why its rules reject the value.
