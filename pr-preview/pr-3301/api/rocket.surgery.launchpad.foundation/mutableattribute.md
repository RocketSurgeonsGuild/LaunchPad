# MutableAttribute

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1253`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3301-merge.Sha.477a5d9575a97343218a11eba2a3d0088808ddab/src/Foundation/MutableAttribute.cs)
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

- [MutableAttribute(string, string)](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.foundation/mutableattribute/constructors.md#constructor-string-string) -- Create the given mutable object

## Properties

- [Prefix](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.foundation/mutableattribute/properties.md#prefix) : `string` `get` -- The prefix
- [Suffix](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.foundation/mutableattribute/properties.md#suffix) : `string` `get` -- The suffix
