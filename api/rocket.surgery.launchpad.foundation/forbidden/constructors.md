# Forbidden Constructors

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/api/rocket.surgery.launchpad.foundation.md)
- Type: [Forbidden](/LaunchPad/api/rocket.surgery.launchpad.foundation/forbidden.md)
- Kind: `Constructors`
- Members: `1`

The caller may not do this. HTTP 403, gRPC `PermissionDenied`, GraphQL `FORBIDDEN`.

<a id="constructor"></a>
<a id="constructor-string"></a>

## Forbidden(string?)

- Name: `Constructor(string?)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.7c2af63fb1eca0ecbe24e62d859d0132ae74c3a6/src/Foundation/Problems.cs#L68)

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
