# Conflict Constructors

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/api/rocket.surgery.launchpad.foundation.md)
- Type: [Conflict](/LaunchPad/api/rocket.surgery.launchpad.foundation/conflict.md)
- Kind: `Constructors`
- Members: `1`

The request conflicts with the current state. HTTP 409, gRPC `FailedPrecondition`, GraphQL `CONFLICT`.

<a id="constructor"></a>
<a id="constructor-string"></a>

## Conflict(string?)

- Name: `Constructor(string?)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.bc6e38a474db052319ef3b044454738f3a433de2/src/Foundation/Problems.cs#L73)

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
