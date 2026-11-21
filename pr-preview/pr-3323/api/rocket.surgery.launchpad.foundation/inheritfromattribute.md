# InheritFromAttribute

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1275`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3323-merge.Sha.848d7df41fb82a23ce0738e64978a35c934d7558/src/Foundation/InheritFromAttribute.cs)
- Inherits: `Attribute`

Allows you copy the properties of the another class or interface, and include a helper method to copy content to the given record.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public sealed class InheritFromAttribute
    : System.Attribute
{
    // ...
}
```

## Constructors

- [InheritFromAttribute(Type)](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.foundation/inheritfromattribute/constructors.md#constructor-type) -- Create the inherit from attribute

## Properties

- [Exclude](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.foundation/inheritfromattribute/properties.md#exclude) : `string[]` `get; init` -- Exclude the given properties from the copy
