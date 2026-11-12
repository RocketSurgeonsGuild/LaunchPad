# IStrongTypeVisitor<TResult> Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives.md)
- Type: [IStrongTypeVisitor<TResult>](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/istrongtypevisitor-1.md)
- Kind: `Methods`
- Members: `1`

Does something with a strong type known by its generic arguments, such as building a converter for it.

<a id="visit"></a>
<a id="visit-strongtypedescription-tstrongtype-tprimitive"></a>

## Visit(StrongTypeDescription<TStrongType, TPrimitive>)

- Name: `Visit(StrongTypeDescription<TStrongType, TPrimitive>)`
- Modifiers: `abstract`
- Returns: `TResult`

Visits one strong type.

```csharp
public interface IStrongTypeVisitor<TResult>
{
    public abstract TResult Visit<TStrongType, TPrimitive>(
        StrongTypeDescription<TStrongType, TPrimitive> description)
    {
        // ...
    }
}
```

## Parameters

- `description` ([StrongTypeDescription<TStrongType, TPrimitive>](/LaunchPad/pr-preview/pr-3192/api/rocket.surgery.launchpad.primitives/strongtypedescription-2.md))
  Its description.

## Returns

`TResult` -- The result.
