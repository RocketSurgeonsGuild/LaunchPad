# UnprocessableEntity Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3170/api/rocket.surgery.launchpad.primitives.md)
- Type: [UnprocessableEntity](/LaunchPad/pr-preview/pr-3170/api/rocket.surgery.launchpad.primitives/unprocessableentity.md)
- Kind: `Properties`
- Members: `1`

The request is well formed but fails validation: what FluentValidation reports. HTTP 422, gRPC `InvalidArgument`, GraphQL `BAD_USER_INPUT`.

<a id="errors"></a>

## Errors

- Name: `Errors`
- Modifiers: `get; init`
- Returns: `IReadOnlyDictionary<string, string[]>`

The errors, by property name.

```csharp
public IReadOnlyDictionary<string, string[]> Errors { get; init; }
```
