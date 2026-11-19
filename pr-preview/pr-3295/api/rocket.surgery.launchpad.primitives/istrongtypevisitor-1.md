# IStrongTypeVisitor<TResult>

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3295/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1245`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3295-merge.Sha.8032622be33212edca749d51eeb2f284c76d51b7/src/Primitives/Descriptions/IStrongTypeVisitor.cs)

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

- [Visit(StrongTypeDescription<TStrongType, TPrimitive>)](/LaunchPad/pr-preview/pr-3295/api/rocket.surgery.launchpad.primitives/istrongtypevisitor-1/methods.md#visit-strongtypedescription-tstrongtype-tprimitive) : `TResult` `abstract` -- Visits one strong type.
