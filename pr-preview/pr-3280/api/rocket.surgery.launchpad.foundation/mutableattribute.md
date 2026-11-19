# MutableAttribute

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3280/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1244`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3280-merge.Sha.d53d78d7adee47674f27c3f3dda0796827a126dc/src/Foundation/MutableAttribute.cs)
- Inherits: `Attribute`

Creates a new class that is a mutable version of the given class.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public sealed class MutableAttribute
    : System.Attribute
{
    // ...
}
```

## Constructors

- [MutableAttribute(string, string)](/LaunchPad/pr-preview/pr-3280/api/rocket.surgery.launchpad.foundation/mutableattribute/constructors.md#constructor-string-string) -- Create the given mutable object

## Properties

- [Prefix](/LaunchPad/pr-preview/pr-3280/api/rocket.surgery.launchpad.foundation/mutableattribute/properties.md#prefix) : `string` `get` -- The prefix
- [Suffix](/LaunchPad/pr-preview/pr-3280/api/rocket.surgery.launchpad.foundation/mutableattribute/properties.md#suffix) : `string` `get` -- The suffix
