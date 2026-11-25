# IDomainPrimitiveVisitor<TResult>

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.454`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives.Descriptions`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.752d0f02a7c5d0592531a44cd02f98c0fc107a63/src/Primitives/Descriptions/IDomainPrimitiveVisitor.cs)

Does something with a domain primitive known by its generic arguments, such as building a converter for it.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives.Descriptions;

public interface IDomainPrimitiveVisitor<TResult>
{
    // ...
}
```

## Type Parameters

- `TResult` -- What the visitor returns.

## Methods

- [Visit(DomainPrimitiveDescription<TPrimitive, TValue>)](/LaunchPad/api/rocket.surgery.launchpad.primitives/idomainprimitivevisitor-1/methods.md#visit-domainprimitivedescription-tprimitive-tvalue) : `TResult` `abstract` -- Visits one domain primitive.
