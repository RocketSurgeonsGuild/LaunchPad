# Invalid Constructors

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.foundation.md)
- Type: [Invalid](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.foundation/invalid.md)
- Kind: `Constructors`
- Members: `1`

The request is malformed or can never succeed as sent. HTTP 400, gRPC `InvalidArgument`, GraphQL `BAD_USER_INPUT`.

<a id="constructor"></a>
<a id="constructor-string"></a>

## Invalid(string?)

- Name: `Constructor(string?)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.11f685f6c8c4739a5c9c37bdc0dec7baf7e3894e/src/Foundation/Problems.cs#L42-L45)

The request is malformed or can never succeed as sent. HTTP 400, gRPC `InvalidArgument`, GraphQL `BAD_USER_INPUT`.

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
