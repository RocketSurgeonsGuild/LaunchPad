# Invalid Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.primitives.md)
- Type: [Invalid](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.primitives/invalid.md)
- Kind: `Properties`
- Members: `1`

The request is invalid. HTTP 400, gRPC `InvalidArgument`, GraphQL `INVALID`.

<a id="errors"></a>

## Errors

- Name: `Errors`
- Modifiers: `get; init`
- Returns: `IReadOnlyDictionary<string, string[]>`

The validation errors, by property name.

```csharp
public IReadOnlyDictionary<string, string[]> Errors { get; init; }
```
