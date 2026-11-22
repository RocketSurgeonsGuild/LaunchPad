# InheritFromAttribute Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3327/api/rocket.surgery.launchpad.foundation.md)
- Type: [InheritFromAttribute](/LaunchPad/pr-preview/pr-3327/api/rocket.surgery.launchpad.foundation/inheritfromattribute.md)
- Kind: `Properties`
- Members: `1`

Allows you copy the properties of the another class or interface, and include a helper method to copy content to the given record.

<a id="exclude"></a>

## Exclude

- Name: `Exclude`
- Modifiers: `get; init`
- Returns: `string[]`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3327-merge.Sha.f618e95e07fca6c7011f5e648f5b4966f567c6b1/src/Foundation/InheritFromAttribute.cs)

Exclude the given properties from the copy

```csharp
public string[] Exclude { get; init; }
```
