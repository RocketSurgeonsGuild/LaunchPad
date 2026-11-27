# AssignedJsonConverter<T> Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3351/api/rocket.surgery.launchpad.foundation.md)
- Type: [AssignedJsonConverter<T>](/LaunchPad/pr-preview/pr-3351/api/rocket.surgery.launchpad.foundation/assignedjsonconverter-1.md)
- Kind: `Properties`
- Members: `1`

Reads and writes an `Foundation.Assigned`1` as the bare `T`: a property that is in the JSON is assigned its value, one that isn't stays unassigned (the property's own `Assigned`1.Empty`).

<a id="handlenull"></a>

## HandleNull

- Name: `HandleNull`
- Modifiers: `override` `get`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3351-merge.Sha.a0ae38aa18a1b9917789ec429da9c9c80b65406f/src/Foundation/AssignedJsonConverter.cs#L28)

Gets a value that indicates whether `null` should be passed to the converter on serialization, and whether `JsonTokenType.Null` should be passed on deserialization.

```csharp
public override bool HandleNull { get; }
```
