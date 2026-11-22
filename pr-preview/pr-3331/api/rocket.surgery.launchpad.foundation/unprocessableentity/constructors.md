# UnprocessableEntity Constructors

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.foundation.md)
- Type: [UnprocessableEntity](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.foundation/unprocessableentity.md)
- Kind: `Constructors`
- Members: `1`

The request is well formed but fails validation: what a validator reports. HTTP 422, gRPC `InvalidArgument`, GraphQL `BAD_USER_INPUT`.

<a id="constructor"></a>
<a id="constructor-string"></a>

## UnprocessableEntity(string?)

- Name: `Constructor(string?)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3331-merge.Sha.4e9f7c28a9b18bc67da869f4b85f2484ef7df001/src/Foundation/Problems.cs#L54-L57)

The request is well formed but fails validation: what a validator reports. HTTP 422, gRPC `InvalidArgument`, GraphQL `BAD_USER_INPUT`.

```csharp
public record UnprocessableEntity
{
    public UnprocessableEntity(
        string? Detail = null)
    {
        // ...
    }
}
```

## Parameters

- `Detail` (`string?`) `optional`
  A human-readable explanation specific to this occurrence of the problem.
