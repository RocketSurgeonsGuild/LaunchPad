# MutableAttribute Constructors

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.foundation.md)
- Type: [MutableAttribute](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.foundation/mutableattribute.md)
- Kind: `Constructors`
- Members: `1`

Creates a new class that is a mutable version of the given class.

<a id="constructor"></a>
<a id="constructor-string-string"></a>

## MutableAttribute(string, string)

- Name: `Constructor(string, string)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3281-merge.Sha.65b042458886c034a574f04a60cfe2fa08ef3e1a/src/Foundation/MutableAttribute.cs#L17-L21)

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
