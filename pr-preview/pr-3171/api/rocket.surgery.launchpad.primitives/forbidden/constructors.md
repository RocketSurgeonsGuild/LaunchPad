# Forbidden Constructors

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3171/api/rocket.surgery.launchpad.primitives.md)
- Type: [Forbidden](/LaunchPad/pr-preview/pr-3171/api/rocket.surgery.launchpad.primitives/forbidden.md)
- Kind: `Constructors`
- Members: `1`

The caller may not do this. HTTP 403, gRPC `PermissionDenied`, GraphQL `FORBIDDEN`.

<a id="constructor"></a>
<a id="constructor-string"></a>

## Forbidden(string?)

- Name: `Constructor(string?)`

The caller may not do this. HTTP 403, gRPC `PermissionDenied`, GraphQL `FORBIDDEN`.

```csharp
public record Forbidden
{
    public Forbidden(
        string? Detail = null)
    {
        // ...
    }
}
```

## Parameters

- `Detail` (`string?`) `optional`
  A human-readable explanation specific to this occurrence of the problem.
