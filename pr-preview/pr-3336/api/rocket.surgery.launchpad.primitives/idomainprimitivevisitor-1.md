# IDomainPrimitiveVisitor<TResult>

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1290`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives.Descriptions`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.7cda342c7a30094b955266549c4928a5c52b02bb/src/Primitives/Descriptions/IDomainPrimitiveVisitor.cs)

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

- [Visit(DomainPrimitiveDescription<TPrimitive, TValue>)](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/idomainprimitivevisitor-1/methods.md#visit-domainprimitivedescription-tprimitive-tvalue) : `TResult` `abstract` -- Visits one domain primitive.
