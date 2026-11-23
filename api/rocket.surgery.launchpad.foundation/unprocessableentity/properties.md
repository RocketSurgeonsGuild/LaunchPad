# UnprocessableEntity Properties

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/api/rocket.surgery.launchpad.foundation.md)
- Type: [UnprocessableEntity](/LaunchPad/api/rocket.surgery.launchpad.foundation/unprocessableentity.md)
- Kind: `Properties`
- Members: `1`

The request is well formed but fails validation: what a validator reports. HTTP 422, gRPC `InvalidArgument`, GraphQL `BAD_USER_INPUT`.

<a id="errors"></a>

## Errors

- Name: `Errors`
- Modifiers: `get; init`
- Returns: `IReadOnlyDictionary<string, string[]>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.54628ef221bd1ad09c6ebc55da16ddcb5027f4c7/src/Foundation/Problems.cs)

The errors, by property name.

```csharp
public IReadOnlyDictionary<string, string[]> Errors { get; init; }
```
