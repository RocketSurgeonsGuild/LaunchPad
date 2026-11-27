# DomainPrimitiveDescription<TPrimitive, TValue> Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3349/api/rocket.surgery.launchpad.primitives.md)
- Type: [DomainPrimitiveDescription<TPrimitive, TValue>](/LaunchPad/pr-preview/pr-3349/api/rocket.surgery.launchpad.primitives/domainprimitivedescription-2.md)
- Kind: `Methods`
- Members: `1`

The description of one domain primitive, which knows the domain primitive and its value as generic arguments.

<a id="accept"></a>
<a id="accept-idomainprimitivevisitor-tresult"></a>

## Accept(IDomainPrimitiveVisitor<TResult>)

- Name: `Accept(IDomainPrimitiveVisitor<TResult>)`
- Modifiers: `override`
- Returns: `TResult`

Hands the domain primitive and its value to `visitor` as generic arguments.

```csharp
public sealed class DomainPrimitiveDescription<TPrimitive, TValue>
{
    public override TResult DomainPrimitiveDescription<TPrimitive, Accept<TResult>(IDomainPrimitiveVisitor<TResult> visitor)
    {
        // ...
    }
}
```

## Parameters

- `visitor` ([IDomainPrimitiveVisitor<TResult>](/LaunchPad/pr-preview/pr-3349/api/rocket.surgery.launchpad.primitives/idomainprimitivevisitor-1.md))
  The visitor.

## Returns

`TResult` -- What the visitor returned.
