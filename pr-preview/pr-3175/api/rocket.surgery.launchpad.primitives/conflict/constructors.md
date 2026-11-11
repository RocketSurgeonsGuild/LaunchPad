# Conflict Constructors

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives.md)
- Type: [Conflict](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.primitives/conflict.md)
- Kind: `Constructors`
- Members: `1`

The request conflicts with the current state. HTTP 409, gRPC `FailedPrecondition`, GraphQL `CONFLICT`.

<a id="constructor"></a>
<a id="constructor-string"></a>

## Conflict(string?)

- Name: `Constructor(string?)`

The request conflicts with the current state. HTTP 409, gRPC `FailedPrecondition`, GraphQL `CONFLICT`.

```csharp
public record Conflict
{
    public Conflict(
        string? Detail = null)
    {
        // ...
    }
}
```

## Parameters

- `Detail` (`string?`) `optional`
  A human-readable explanation specific to this occurrence of the problem.
