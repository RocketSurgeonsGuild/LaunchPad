# Unauthorized Constructors

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3317/api/rocket.surgery.launchpad.foundation.md)
- Type: [Unauthorized](/LaunchPad/pr-preview/pr-3317/api/rocket.surgery.launchpad.foundation/unauthorized.md)
- Kind: `Constructors`
- Members: `1`

The caller isn't authenticated. HTTP 401, gRPC `Unauthenticated`, GraphQL `UNAUTHENTICATED`.

<a id="constructor"></a>
<a id="constructor-string"></a>

## Unauthorized(string?)

- Name: `Constructor(string?)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3317-merge.Sha.125c9fda2ca8a5e1aa1ca7ddbbdcadbcdb967166/src/Foundation/Problems.cs#L63)

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
