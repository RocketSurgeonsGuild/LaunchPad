# ValueObjectMemberDescription Constructors

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives.md)
- Type: [ValueObjectMemberDescription](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription.md)
- Kind: `Constructors`
- Members: `1`

One value a value object holds.

<a id="constructor"></a>
<a id="constructor-string-type-bool"></a>

## ValueObjectMemberDescription(string, Type, bool)

- Name: `Constructor(string, Type, bool)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3337-merge.Sha.d457558b817469f413055201c091b86e2edbfa9a/src/Primitives/Descriptions/DomainPrimitiveDescription.cs#L167)

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
