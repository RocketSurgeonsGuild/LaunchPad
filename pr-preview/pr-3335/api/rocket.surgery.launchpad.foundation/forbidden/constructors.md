# Forbidden Constructors

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3335/api/rocket.surgery.launchpad.foundation.md)
- Type: [Forbidden](/LaunchPad/pr-preview/pr-3335/api/rocket.surgery.launchpad.foundation/forbidden.md)
- Kind: `Constructors`
- Members: `1`

The caller may not do this. HTTP 403, gRPC `PermissionDenied`, GraphQL `FORBIDDEN`.

<a id="constructor"></a>
<a id="constructor-string"></a>

## Forbidden(string?)

- Name: `Constructor(string?)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3335-merge.Sha.164159712cc333d1205a8448b12272b1815ca0c1/src/Foundation/Problems.cs#L68)

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
