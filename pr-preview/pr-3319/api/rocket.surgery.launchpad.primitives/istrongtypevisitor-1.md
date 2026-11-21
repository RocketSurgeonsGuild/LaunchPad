# IStrongTypeVisitor<TResult>

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3319/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1271`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3319-merge.Sha.db5ef919c4f6a03a7f33e4672dc70757d169d5a4/src/Primitives/Descriptions/IStrongTypeVisitor.cs)

Does something with a strong type known by its generic arguments, such as building a converter for it.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public interface IStrongTypeVisitor<TResult>
{
    // ...
}
```

## Type Parameters

- `TResult` -- What the visitor returns.

## Methods

- [Visit(StrongTypeDescription<TStrongType, TPrimitive>)](/LaunchPad/pr-preview/pr-3319/api/rocket.surgery.launchpad.primitives/istrongtypevisitor-1/methods.md#visit-strongtypedescription-tstrongtype-tprimitive) : `TResult` `abstract` -- Visits one strong type.
