# Unauthorized Constructors

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3144/api/rocket.surgery.launchpad.primitives.md)
- Type: [Unauthorized](/LaunchPad/pr-preview/pr-3144/api/rocket.surgery.launchpad.primitives/unauthorized.md)
- Kind: `Constructors`
- Members: `1`

The caller isn't authenticated. HTTP 401, gRPC `Unauthenticated`, GraphQL `UNAUTHENTICATED`.

<a id="constructor"></a>
<a id="constructor-string"></a>

## Unauthorized(string?)

- Name: `Constructor(string?)`

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
