# IValueObject<TSelf> Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives.md)
- Type: [IValueObject<TSelf>](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/ivalueobject-1.md)
- Kind: `Properties`
- Members: `1`

A value object: a domain primitive that holds several values, validated together.

<a id="description"></a>

## Description

- Name: `Description`
- Modifiers: `static` `abstract` `get`
- Returns: [ValueObjectDescription](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/valueobjectdescription.md)

What an integration needs to know about the value object: the values it holds.

```csharp
public static abstract ValueObjectDescription Description { get; }
```
