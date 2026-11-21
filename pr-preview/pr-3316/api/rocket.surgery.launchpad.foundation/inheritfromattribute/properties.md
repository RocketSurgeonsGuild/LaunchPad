# InheritFromAttribute Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3316/api/rocket.surgery.launchpad.foundation.md)
- Type: [InheritFromAttribute](/LaunchPad/pr-preview/pr-3316/api/rocket.surgery.launchpad.foundation/inheritfromattribute.md)
- Kind: `Properties`
- Members: `1`

Allows you copy the properties of the another class or interface, and include a helper method to copy content to the given record.

<a id="exclude"></a>

## Exclude

- Name: `Exclude`
- Modifiers: `get; init`
- Returns: `string[]`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3316-merge.Sha.5b7e33a497070ffcf95e9f2c28132b3e4913cfd6/src/Foundation/InheritFromAttribute.cs)

Exclude the given properties from the copy

```csharp
public string[] Exclude { get; init; }
```
