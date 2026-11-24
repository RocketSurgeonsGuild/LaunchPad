# IDomainPrimitiveVisitor<TResult> Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Type: [IDomainPrimitiveVisitor<TResult>](/LaunchPad/api/rocket.surgery.launchpad.primitives/idomainprimitivevisitor-1.md)
- Kind: `Methods`
- Members: `1`

Does something with a domain primitive known by its generic arguments, such as building a converter for it.

<a id="visit"></a>
<a id="visit-domainprimitivedescription-tprimitive-tvalue"></a>

## Visit(DomainPrimitiveDescription<TPrimitive, TValue>)

- Name: `Visit(DomainPrimitiveDescription<TPrimitive, TValue>)`
- Modifiers: `abstract`
- Returns: `TResult`

Visits one domain primitive.

```csharp
public interface IDomainPrimitiveVisitor<TResult>
{
    public abstract TResult Visit<TPrimitive, TValue>(
        DomainPrimitiveDescription<TPrimitive, TValue> description)
    {
        // ...
    }
}
```

## Parameters

- `description` ([DomainPrimitiveDescription<TPrimitive, TValue>](/LaunchPad/api/rocket.surgery.launchpad.primitives/domainprimitivedescription-2.md))
  Its description.

## Returns

`TResult` -- The result.
