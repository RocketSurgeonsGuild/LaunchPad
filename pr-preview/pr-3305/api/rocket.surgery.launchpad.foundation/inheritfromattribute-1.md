# InheritFromAttribute<T>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3305/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1256`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3305-merge.Sha.f536a04ca4e28fbdc06da7c1e0f968bbeccd9e9a/src/Foundation/InheritFromAttribute.cs)
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

- [InheritFromAttribute](/LaunchPad/pr-preview/pr-3305/api/rocket.surgery.launchpad.foundation/inheritfromattribute-1/constructors.md#constructor) -- Create the inherit from attribute

## Properties

- [Exclude](/LaunchPad/pr-preview/pr-3305/api/rocket.surgery.launchpad.foundation/inheritfromattribute-1/properties.md#exclude) : `string[]` `get; init` -- Exclude the given properties from the copy
