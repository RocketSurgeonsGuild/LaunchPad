# MutableAttribute Constructors

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/api/rocket.surgery.launchpad.foundation.md)
- Type: [MutableAttribute](/LaunchPad/api/rocket.surgery.launchpad.foundation/mutableattribute.md)
- Kind: `Constructors`
- Members: `1`

Creates a new class that is a mutable version of the given class.

<a id="constructor"></a>
<a id="constructor-string-string"></a>

## MutableAttribute(string, string)

- Name: `Constructor(string, string)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.84416b61eeb4534ffbcb255bb4d7cc5d6872eac9/src/Foundation/MutableAttribute.cs#L17-L21)

Create the given mutable object

```csharp
public sealed class MutableAttribute
{
    public MutableAttribute(
        string prefix = "",
        string suffix = "ViewModel")
    {
        // ...
    }
}
```

## Parameters

- `prefix` (`string`) `optional`
- `suffix` (`string`) `optional`
