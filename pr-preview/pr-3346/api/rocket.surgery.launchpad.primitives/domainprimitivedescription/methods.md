# DomainPrimitiveDescription Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3346/api/rocket.surgery.launchpad.primitives.md)
- Type: [DomainPrimitiveDescription](/LaunchPad/pr-preview/pr-3346/api/rocket.surgery.launchpad.primitives/domainprimitivedescription.md)
- Kind: `Methods`
- Members: `1`

What an integration needs to know about one domain primitive that holds one value: its value type, the format it is written in, and the constraints of its rules (lengths, patterns, bounds, allowed values). A rule written as code ( `[CustomValidator<T>]`) is not described.

<a id="accept"></a>
<a id="accept-idomainprimitivevisitor-tresult"></a>

## Accept(IDomainPrimitiveVisitor<TResult>)

- Name: `Accept(IDomainPrimitiveVisitor<TResult>)`
- Modifiers: `abstract`
- Returns: `TResult`

Hands the domain primitive and its value to `visitor` as generic arguments.

```csharp
public abstract class DomainPrimitiveDescription
{
    public abstract TResult Accept<TResult>(
        IDomainPrimitiveVisitor<TResult> visitor)
    {
        // ...
    }
}
```

## Parameters

- `visitor` ([IDomainPrimitiveVisitor<TResult>](/LaunchPad/pr-preview/pr-3346/api/rocket.surgery.launchpad.primitives/idomainprimitivevisitor-1.md))
  The visitor.

## Returns

`TResult` -- What the visitor returned.
