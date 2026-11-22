# InheritFromAttribute<T> Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3329/api/rocket.surgery.launchpad.foundation.md)
- Type: [InheritFromAttribute<T>](/LaunchPad/pr-preview/pr-3329/api/rocket.surgery.launchpad.foundation/inheritfromattribute-1.md)
- Kind: `Properties`
- Members: `1`

Allows you copy the properties of the another class or interface, and include a helper method to copy content to the given record.

<a id="exclude"></a>

## Exclude

- Name: `Exclude`
- Modifiers: `get; init`
- Returns: `string[]`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3329-merge.Sha.a2bf3f292dae7cdf0854fdc32bf25e9839e7110d/src/Foundation/InheritFromAttribute.cs)

Exclude the given properties from the copy

```csharp
public string[] Exclude { get; init; }
```
