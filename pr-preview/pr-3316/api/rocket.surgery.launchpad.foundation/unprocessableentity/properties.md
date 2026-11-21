# UnprocessableEntity Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3316/api/rocket.surgery.launchpad.foundation.md)
- Type: [UnprocessableEntity](/LaunchPad/pr-preview/pr-3316/api/rocket.surgery.launchpad.foundation/unprocessableentity.md)
- Kind: `Properties`
- Members: `1`

The request is well formed but fails validation: what a validator reports. HTTP 422, gRPC `InvalidArgument`, GraphQL `BAD_USER_INPUT`.

<a id="errors"></a>

## Errors

- Name: `Errors`
- Modifiers: `get; init`
- Returns: `IReadOnlyDictionary<string, string[]>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3316-merge.Sha.5b7e33a497070ffcf95e9f2c28132b3e4913cfd6/src/Foundation/Problems.cs)

The errors, by property name.

```csharp
public IReadOnlyDictionary<string, string[]> Errors { get; init; }
```
