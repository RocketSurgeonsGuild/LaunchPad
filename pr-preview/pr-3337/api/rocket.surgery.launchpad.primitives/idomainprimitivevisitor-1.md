# IDomainPrimitiveVisitor<TResult>

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1331`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives.Descriptions`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3337-merge.Sha.8c1925d684bc0cbcfb962e8486f837c5164aa6f2/src/Primitives/Descriptions/IDomainPrimitiveVisitor.cs)

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

- [Visit(DomainPrimitiveDescription<TPrimitive, TValue>)](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/idomainprimitivevisitor-1/methods.md#visit-domainprimitivedescription-tprimitive-tvalue) : `TResult` `abstract` -- Visits one domain primitive.
