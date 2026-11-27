# NotFound Constructors

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/api/rocket.surgery.launchpad.foundation.md)
- Type: [NotFound](/LaunchPad/api/rocket.surgery.launchpad.foundation/notfound.md)
- Kind: `Constructors`
- Members: `1`

The requested entity doesn't exist. HTTP 404, gRPC `NotFound`, GraphQL `NOT_FOUND`.

<a id="constructor"></a>
<a id="constructor-string"></a>

## NotFound(string?)

- Name: `Constructor(string?)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.cd7db14e49b12478b13b2f21edf214c613078b4c/src/Foundation/Problems.cs#L37)

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
