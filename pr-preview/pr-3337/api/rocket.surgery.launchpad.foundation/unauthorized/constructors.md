# Unauthorized Constructors

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.foundation.md)
- Type: [Unauthorized](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.foundation/unauthorized.md)
- Kind: `Constructors`
- Members: `1`

The caller isn't authenticated. HTTP 401, gRPC `Unauthenticated`, GraphQL `UNAUTHENTICATED`.

<a id="constructor"></a>
<a id="constructor-string"></a>

## Unauthorized(string?)

- Name: `Constructor(string?)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3337-merge.Sha.b3468f8f7f271d2dc63ee1d3333c2bcdd48e93a2/src/Foundation/Problems.cs#L63)

The caller isn't authenticated. HTTP 401, gRPC `Unauthenticated`, GraphQL `UNAUTHENTICATED`.

```csharp
public record Unauthorized
{
    public Unauthorized(
        string? Detail = null)
    {
        // ...
    }
}
```

## Parameters

- `Detail` (`string?`) `optional`
  A human-readable explanation specific to this occurrence of the problem.
