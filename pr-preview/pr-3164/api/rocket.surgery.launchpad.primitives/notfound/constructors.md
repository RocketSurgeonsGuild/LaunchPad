# NotFound Constructors

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.primitives.md)
- Type: [NotFound](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.primitives/notfound.md)
- Kind: `Constructors`
- Members: `1`

The requested entity doesn't exist. HTTP 404, gRPC `NotFound`, GraphQL `NOT_FOUND`.

<a id="constructor"></a>
<a id="constructor-string"></a>

## NotFound(string?)

- Name: `Constructor(string?)`

The requested entity doesn't exist. HTTP 404, gRPC `NotFound`, GraphQL `NOT_FOUND`.

```csharp
public record NotFound
{
    public NotFound(
        string? Detail = null)
    {
        // ...
    }
}
```

## Parameters

- `Detail` (`string?`) `optional`
  A human-readable explanation specific to this occurrence of the problem.
