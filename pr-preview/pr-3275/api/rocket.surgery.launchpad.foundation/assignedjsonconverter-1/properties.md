# AssignedJsonConverter<T> Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3275/api/rocket.surgery.launchpad.foundation.md)
- Type: [AssignedJsonConverter<T>](/LaunchPad/pr-preview/pr-3275/api/rocket.surgery.launchpad.foundation/assignedjsonconverter-1.md)
- Kind: `Properties`
- Members: `1`

Reads and writes an `Foundation.Assigned`1` as the bare `T`: a property that is in the JSON is assigned its value, one that isn't stays unassigned (the property's own `Assigned`1.Empty`).

<a id="handlenull"></a>

## HandleNull

- Name: `HandleNull`
- Modifiers: `override` `get`
- Returns: `bool`

Gets a value that indicates whether `null` should be passed to the converter on serialization, and whether `JsonTokenType.Null` should be passed on deserialization.

```csharp
public override bool HandleNull { get; }
```
