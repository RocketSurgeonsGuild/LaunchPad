# IDomainPrimitive<TSelf>

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1300`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3347-merge.Sha.c2b6537b4ab49f853c50fea9f1114836c5d8adf2/src/Primitives/IDomainPrimitive.cs)

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

- [IsDefault](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.primitives/idomainprimitive-1/properties.md#isdefault) : `bool` `virtual` `get` -- Whether this is the default value of a domain primitive declared as a struct, which holds no value. A domain primitive declared as a class is never default: a missing value is `null`.
- [PrimitiveName](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.primitives/idomainprimitive-1/properties.md#primitivename) : `string` `static` `abstract` `get` -- The name of the domain primitive, used in errors and diagnostics.
