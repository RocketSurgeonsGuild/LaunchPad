# Invalid Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.foundation.md)
- Type: [Invalid](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.foundation/invalid.md)
- Kind: `Properties`
- Members: `1`

The request is malformed or can never succeed as sent. HTTP 400, gRPC `InvalidArgument`, GraphQL `BAD_USER_INPUT`.

<a id="errors"></a>

## Errors

- Name: `Errors`
- Modifiers: `get; init`
- Returns: `IReadOnlyDictionary<string, string[]>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3320-merge.Sha.bb93b2eb5e069135a2caef17855a85e8a18d22af/src/Foundation/Problems.cs)

The errors, by property name.

```csharp
public IReadOnlyDictionary<string, string[]> Errors { get; init; }
```
