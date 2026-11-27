# ValueObjectMemberDescription Constructors

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Type: [ValueObjectMemberDescription](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription.md)
- Kind: `Constructors`
- Members: `1`

One value a value object holds.

<a id="constructor"></a>
<a id="constructor-string-type-bool"></a>

## ValueObjectMemberDescription(string, Type, bool)

- Name: `Constructor(string, Type, bool)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.cd7db14e49b12478b13b2f21edf214c613078b4c/src/Primitives/Descriptions/DomainPrimitiveDescription.cs#L167)

One value a value object holds.

```csharp
public record ValueObjectMemberDescription
{
    public ValueObjectMemberDescription(
        string Name,
        Type Type,
        bool IsNullable)
    {
        // ...
    }
}
```

## Parameters

- `Name` (`string`)
  The name of its property.
- `Type` (`Type`)
  The type of the value.
- `IsNullable` (`bool`)
  Whether the value may be missing (a nullable reference or value type).
