# Unauthorized Constructors

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/api/rocket.surgery.launchpad.foundation.md)
- Type: [Unauthorized](/LaunchPad/api/rocket.surgery.launchpad.foundation/unauthorized.md)
- Kind: `Constructors`
- Members: `1`

The caller isn't authenticated. HTTP 401, gRPC `Unauthenticated`, GraphQL `UNAUTHENTICATED`.

<a id="constructor"></a>
<a id="constructor-string"></a>

## Unauthorized(string?)

- Name: `Constructor(string?)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.42409360cf41d0e5a89b31252cf8cea56b207c47/src/Foundation/Problems.cs#L63)

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
