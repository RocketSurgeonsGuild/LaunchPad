# MutableAttribute Constructors

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3299/api/rocket.surgery.launchpad.foundation.md)
- Type: [MutableAttribute](/LaunchPad/pr-preview/pr-3299/api/rocket.surgery.launchpad.foundation/mutableattribute.md)
- Kind: `Constructors`
- Members: `1`

Creates a new class that is a mutable version of the given class.

<a id="constructor"></a>
<a id="constructor-string-string"></a>

## MutableAttribute(string, string)

- Name: `Constructor(string, string)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3299-merge.Sha.307aa7d68ee152c9493e88e21694727a4cdb86e8/src/Foundation/MutableAttribute.cs#L17-L21)

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
