# InheritFromAttribute Constructors

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3310/api/rocket.surgery.launchpad.foundation.md)
- Type: [InheritFromAttribute](/LaunchPad/pr-preview/pr-3310/api/rocket.surgery.launchpad.foundation/inheritfromattribute.md)
- Kind: `Constructors`
- Members: `1`

Allows you copy the properties of the another class or interface, and include a helper method to copy content to the given record.

<a id="constructor"></a>
<a id="constructor-type"></a>

## InheritFromAttribute(Type)

- Name: `Constructor(Type)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3310-merge.Sha.8860010d83b5d02194514237744998b27c36d06d/src/Foundation/InheritFromAttribute.cs#L17-L22)

Create the inherit from attribute

```csharp
public sealed class InheritFromAttribute
{
    public InheritFromAttribute(
        Type classToCopy)
    {
        // ...
    }
}
```

## Parameters

- `classToCopy` (`Type`)
