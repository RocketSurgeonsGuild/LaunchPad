# InheritFromAttribute<T> Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3346/api/rocket.surgery.launchpad.foundation.md)
- Type: [InheritFromAttribute<T>](/LaunchPad/pr-preview/pr-3346/api/rocket.surgery.launchpad.foundation/inheritfromattribute-1.md)
- Kind: `Properties`
- Members: `1`

Allows you copy the properties of the another class or interface, and include a helper method to copy content to the given record.

<a id="exclude"></a>

## Exclude

- Name: `Exclude`
- Modifiers: `get; init`
- Returns: `string[]`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3346-merge.Sha.ddf4e8df3db056914fd776aa443a00dc7536dd57/src/Foundation/InheritFromAttribute.cs)

Exclude the given properties from the copy

```csharp
public string[] Exclude { get; init; }
```
