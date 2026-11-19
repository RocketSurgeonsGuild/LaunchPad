# MutableAttribute

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-beta.416`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.7c2af63fb1eca0ecbe24e62d859d0132ae74c3a6/src/Foundation/MutableAttribute.cs)
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

- [MutableAttribute(string, string)](/LaunchPad/api/rocket.surgery.launchpad.foundation/mutableattribute/constructors.md#constructor-string-string) -- Create the given mutable object

## Properties

- [Prefix](/LaunchPad/api/rocket.surgery.launchpad.foundation/mutableattribute/properties.md#prefix) : `string` `get` -- The prefix
- [Suffix](/LaunchPad/api/rocket.surgery.launchpad.foundation/mutableattribute/properties.md#suffix) : `string` `get` -- The suffix
