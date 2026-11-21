# ValidatedByAttribute

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1272`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3320-merge.Sha.bb93b2eb5e069135a2caef17855a85e8a18d22af/src/Foundation/ValidatedByAttribute.cs)
- Inherits: `Attribute`

Names the validator of the type it marks, for a form validator to create when no validator of the type is registered. The [MutableAttribute](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.foundation/mutableattribute.md) generator puts it on a view model, naming the validator it writes: that validator checks the value the view model makes, so the form binds to the view model and the rules of the type it copies apply.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public sealed class ValidatedByAttribute
    : System.Attribute
{
    // ...
}
```

## Constructors

- [ValidatedByAttribute(Type)](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.foundation/validatedbyattribute/constructors.md#constructor-type) -- Names the validator of the type it marks, for a form validator to create when no validator of the type is registered. The [MutableAttribute](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.foundation/mutableattribute.md) generator puts it on a view model, naming the validator it writes: that validator checks the value the view model makes, so the form binds to the view model and the rules of the type it copies apply.

## Properties

- [ValidatorType](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.foundation/validatedbyattribute/properties.md#validatortype) : `Type` `get` -- The validator of the type.
