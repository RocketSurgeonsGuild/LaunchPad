# ValueObjectDescription Constructors

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.primitives.md)
- Type: [ValueObjectDescription](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.primitives/valueobjectdescription.md)
- Kind: `Constructors`
- Members: `1`

What an integration needs to know about one value object: the type and the values it holds, in declaration order.

<a id="constructor"></a>
<a id="constructor-type-ireadonlylist-string"></a>

## ValueObjectDescription(Type, IReadOnlyList<string>)

- Name: `Constructor(Type, IReadOnlyList<string>)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3347-merge.Sha.c2b6537b4ab49f853c50fea9f1114836c5d8adf2/src/Primitives/Descriptions/DomainPrimitiveDescription.cs#L151-L157)

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
