# InheritFromAttribute<T> Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3340/api/rocket.surgery.launchpad.foundation.md)
- Type: [InheritFromAttribute<T>](/LaunchPad/pr-preview/pr-3340/api/rocket.surgery.launchpad.foundation/inheritfromattribute-1.md)
- Kind: `Properties`
- Members: `1`

Allows you copy the properties of the another class or interface, and include a helper method to copy content to the given record.

<a id="exclude"></a>

## Exclude

- Name: `Exclude`
- Modifiers: `get; init`
- Returns: `string[]`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3340-merge.Sha.1fd0124e0c52f2b8f0975bb6854b707a6a798292/src/Foundation/InheritFromAttribute.cs)

Exclude the given properties from the copy

```csharp
public string[] Exclude { get; init; }
```
