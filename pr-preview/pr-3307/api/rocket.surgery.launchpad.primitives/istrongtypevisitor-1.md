# IStrongTypeVisitor<TResult>

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1259`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3307-merge.Sha.0d6c4ca8b5b3c5055adee085d785c5712bbc3314/src/Primitives/Descriptions/IStrongTypeVisitor.cs)

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

- [Visit(StrongTypeDescription<TStrongType, TPrimitive>)](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.primitives/istrongtypevisitor-1/methods.md#visit-strongtypedescription-tstrongtype-tprimitive) : `TResult` `abstract` -- Visits one strong type.
