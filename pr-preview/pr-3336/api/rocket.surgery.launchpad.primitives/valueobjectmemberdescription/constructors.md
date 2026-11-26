# ValueObjectMemberDescription Constructors

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives.md)
- Type: [ValueObjectMemberDescription](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription.md)
- Kind: `Constructors`
- Members: `1`

One value a value object holds.

<a id="constructor"></a>
<a id="constructor-string-type-bool"></a>

## ValueObjectMemberDescription(string, Type, bool)

- Name: `Constructor(string, Type, bool)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.9ec91d866bcac66448bc0f439b45a4702f1b73d3/src/Primitives/Descriptions/DomainPrimitiveDescription.cs#L167)

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
