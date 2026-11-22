# UnprocessableEntity Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3327/api/rocket.surgery.launchpad.foundation.md)
- Type: [UnprocessableEntity](/LaunchPad/pr-preview/pr-3327/api/rocket.surgery.launchpad.foundation/unprocessableentity.md)
- Kind: `Properties`
- Members: `1`

The request is well formed but fails validation: what a validator reports. HTTP 422, gRPC `InvalidArgument`, GraphQL `BAD_USER_INPUT`.

<a id="errors"></a>

## Errors

- Name: `Errors`
- Modifiers: `get; init`
- Returns: `IReadOnlyDictionary<string, string[]>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3327-merge.Sha.f618e95e07fca6c7011f5e648f5b4966f567c6b1/src/Foundation/Problems.cs)

The errors, by property name.

```csharp
public IReadOnlyDictionary<string, string[]> Errors { get; init; }
```
