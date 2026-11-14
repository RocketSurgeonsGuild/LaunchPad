# InheritFromAttribute<T>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1183`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net10.0`
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

- [InheritFromAttribute](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.foundation/inheritfromattribute-1/constructors.md#constructor) -- Create the inherit from attribute

## Properties

- [Exclude](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.foundation/inheritfromattribute-1/properties.md#exclude) : `string[]` `get; init` -- Exclude the given properties from the copy
