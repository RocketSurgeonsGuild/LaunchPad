# ValueObjectDescription Constructors

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives.md)
- Type: [ValueObjectDescription](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/valueobjectdescription.md)
- Kind: `Constructors`
- Members: `1`

What an integration needs to know about one value object: the type and the values it holds, in declaration order.

<a id="constructor"></a>
<a id="constructor-type-ireadonlylist-string"></a>

## ValueObjectDescription(Type, IReadOnlyList<string>)

- Name: `Constructor(Type, IReadOnlyList<string>)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.52b728c7d35f7e5541d5238c567b2b39c033d3c7/src/Primitives/Descriptions/DomainPrimitiveDescription.cs#L151-L157)

What an integration needs to know about one value object: the type and the values it holds, in declaration order.

```csharp
public record ValueObjectDescription
{
    public ValueObjectDescription(
        Type Type,
        IReadOnlyList<string> Properties)
    {
        // ...
    }
}
```

## Parameters

- `Type` (`Type`)
  The value object.
- `Properties` (`IReadOnlyList<string>`)
  The names of the values it holds.
