# IDomainPrimitive<TSelf>

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.459`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.8446cfb49f81338ce4d090fef0bb3afa0711fa3f/src/Primitives/IDomainPrimitive.cs)

A domain primitive: a type the Primitives generator completes, which only holds values its rules accept.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives;

public interface IDomainPrimitive<TSelf>
    where TSelf : IDomainPrimitive<TSelf>
{
    // ...
}
```

## Type Parameters

- `TSelf` -- The domain primitive.

## Properties

- [IsDefault](/LaunchPad/api/rocket.surgery.launchpad.primitives/idomainprimitive-1/properties.md#isdefault) : `bool` `virtual` `get` -- Whether this is the default value of a domain primitive declared as a struct, which holds no value. A domain primitive declared as a class is never default: a missing value is `null`.
- [PrimitiveName](/LaunchPad/api/rocket.surgery.launchpad.primitives/idomainprimitive-1/properties.md#primitivename) : `string` `static` `abstract` `get` -- The name of the domain primitive, used in errors and diagnostics.
