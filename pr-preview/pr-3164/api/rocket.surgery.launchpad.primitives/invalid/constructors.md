# Invalid Constructors

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.primitives.md)
- Type: [Invalid](/LaunchPad/pr-preview/pr-3164/api/rocket.surgery.launchpad.primitives/invalid.md)
- Kind: `Constructors`
- Members: `1`

The request is invalid. HTTP 400, gRPC `InvalidArgument`, GraphQL `INVALID`.

<a id="constructor"></a>
<a id="constructor-string"></a>

## Invalid(string?)

- Name: `Constructor(string?)`

The request is invalid. HTTP 400, gRPC `InvalidArgument`, GraphQL `INVALID`.

```csharp
public record Invalid
{
    public Invalid(
        string? Detail = null)
    {
        // ...
    }
}
```

## Parameters

- `Detail` (`string?`) `optional`
  A human-readable explanation specific to this occurrence of the problem.
