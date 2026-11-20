# InheritFromAttribute Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3303/api/rocket.surgery.launchpad.foundation.md)
- Type: [InheritFromAttribute](/LaunchPad/pr-preview/pr-3303/api/rocket.surgery.launchpad.foundation/inheritfromattribute.md)
- Kind: `Properties`
- Members: `1`

Allows you copy the properties of the another class or interface, and include a helper method to copy content to the given record.

<a id="exclude"></a>

## Exclude

- Name: `Exclude`
- Modifiers: `get; init`
- Returns: `string[]`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3303-merge.Sha.af4f47555b5ed577aca4ba95e6332df625343e98/src/Foundation/InheritFromAttribute.cs)

Exclude the given properties from the copy

```csharp
public string[] Exclude { get; init; }
```
