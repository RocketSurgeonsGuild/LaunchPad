# StrongTypeDescription<TStrongType, TPrimitive> Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives.md)
- Type: [StrongTypeDescription<TStrongType, TPrimitive>](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives/strongtypedescription-2.md)
- Kind: `Methods`
- Members: `1`

The description of one strong type, which knows the strong type and its value as generic arguments.

<a id="accept"></a>
<a id="accept-istrongtypevisitor-tresult"></a>

## Accept(IStrongTypeVisitor<TResult>)

- Name: `Accept(IStrongTypeVisitor<TResult>)`
- Modifiers: `override`
- Returns: `TResult`

Hands the strong type and its value to `visitor` as generic arguments.

```csharp
public sealed class StrongTypeDescription<TStrongType, TPrimitive>
{
    public override TResult StrongTypeDescription<TStrongType, Accept<TResult>(IStrongTypeVisitor<TResult> visitor)
    {
        // ...
    }
}
```

## Parameters

- `visitor` ([IStrongTypeVisitor<TResult>](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives/istrongtypevisitor-1.md))
  The visitor.

## Returns

`TResult` -- What the visitor returned.
