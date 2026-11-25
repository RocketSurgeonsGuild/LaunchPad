# ValueObjectDescription Constructors

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Type: [ValueObjectDescription](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectdescription.md)
- Kind: `Constructors`
- Members: `1`

What an integration needs to know about one value object: the type and the values it holds, in declaration order.

<a id="constructor"></a>
<a id="constructor-type-ireadonlylist-string"></a>

## ValueObjectDescription(Type, IReadOnlyList<string>)

- Name: `Constructor(Type, IReadOnlyList<string>)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.752d0f02a7c5d0592531a44cd02f98c0fc107a63/src/Primitives/Descriptions/DomainPrimitiveDescription.cs#L151-L157)

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
