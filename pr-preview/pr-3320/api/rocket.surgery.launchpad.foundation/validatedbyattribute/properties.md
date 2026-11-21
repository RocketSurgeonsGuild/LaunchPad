# ValidatedByAttribute Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.foundation.md)
- Type: [ValidatedByAttribute](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.foundation/validatedbyattribute.md)
- Kind: `Properties`
- Members: `1`

Names the validator of the type it marks, for a form validator to create when no validator of the type is registered. The [MutableAttribute](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.foundation/mutableattribute.md) generator puts it on a view model, naming the validator it writes: that validator checks the value the view model makes, so the form binds to the view model and the rules of the type it copies apply.

<a id="validatortype"></a>

## ValidatorType

- Name: `ValidatorType`
- Modifiers: `get`
- Returns: `Type`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3320-merge.Sha.bb93b2eb5e069135a2caef17855a85e8a18d22af/src/Foundation/ValidatedByAttribute.cs#L20)

The validator of the type.

```csharp
public Type ValidatorType { get; }
```
