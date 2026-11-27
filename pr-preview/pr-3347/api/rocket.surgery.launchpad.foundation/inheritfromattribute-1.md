# InheritFromAttribute<T>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1300`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3347-merge.Sha.c2b6537b4ab49f853c50fea9f1114836c5d8adf2/src/Foundation/InheritFromAttribute.cs)
- Inherits: `Attribute`

Allows you copy the properties of the another class or interface, and include a helper method to copy content to the given record.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public sealed class InheritFromAttribute<T>
    : System.Attribute
    where T : notnull
{
    // ...
}
```

## Constructors

- [InheritFromAttribute](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.foundation/inheritfromattribute-1/constructors.md#constructor) -- Create the inherit from attribute

## Properties

- [Exclude](/LaunchPad/pr-preview/pr-3347/api/rocket.surgery.launchpad.foundation/inheritfromattribute-1/properties.md#exclude) : `string[]` `get; init` -- Exclude the given properties from the copy
