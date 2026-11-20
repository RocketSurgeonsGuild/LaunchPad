# InheritFromAttribute

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1259`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3307-merge.Sha.0d6c4ca8b5b3c5055adee085d785c5712bbc3314/src/Foundation/InheritFromAttribute.cs)
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

- [InheritFromAttribute(Type)](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.foundation/inheritfromattribute/constructors.md#constructor-type) -- Create the inherit from attribute

## Properties

- [Exclude](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.foundation/inheritfromattribute/properties.md#exclude) : `string[]` `get; init` -- Exclude the given properties from the copy
