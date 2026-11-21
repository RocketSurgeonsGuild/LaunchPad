# AssignedJsonConverter<T> Constructors

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.foundation.md)
- Type: [AssignedJsonConverter<T>](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.foundation/assignedjsonconverter-1.md)
- Kind: `Constructors`
- Members: `1`

Reads and writes an `Foundation.Assigned`1` as the bare `T`: a property that is in the JSON is assigned its value, one that isn't stays unassigned (the property's own `Assigned`1.Empty`).

<a id="constructor"></a>

## AssignedJsonConverter

- Name: `Constructor`

```csharp
public sealed class AssignedJsonConverter<T>
{
    public AssignedJsonConverter()
    {
        // ...
    }
}
```
