# InheritFromAttribute

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3348/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1305`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3348-merge.Sha.90dd4871ffef6c7ec9cccc3065d35dfa4da5b954/src/Foundation/InheritFromAttribute.cs)
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

- [InheritFromAttribute(Type)](/LaunchPad/pr-preview/pr-3348/api/rocket.surgery.launchpad.foundation/inheritfromattribute/constructors.md#constructor-type) -- Create the inherit from attribute

## Properties

- [Exclude](/LaunchPad/pr-preview/pr-3348/api/rocket.surgery.launchpad.foundation/inheritfromattribute/properties.md#exclude) : `string[]` `get; init` -- Exclude the given properties from the copy
