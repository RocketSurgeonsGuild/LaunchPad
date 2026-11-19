# MutableAttribute Constructors

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3294/api/rocket.surgery.launchpad.foundation.md)
- Type: [MutableAttribute](/LaunchPad/pr-preview/pr-3294/api/rocket.surgery.launchpad.foundation/mutableattribute.md)
- Kind: `Constructors`
- Members: `1`

Creates a new class that is a mutable version of the given class.

<a id="constructor"></a>
<a id="constructor-string-string"></a>

## MutableAttribute(string, string)

- Name: `Constructor(string, string)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3294-merge.Sha.8ed83894fba92c2ba970626558f3059478959109/src/Foundation/MutableAttribute.cs#L17-L21)

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
