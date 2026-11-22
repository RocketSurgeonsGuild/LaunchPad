# Invalid Constructors

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3327/api/rocket.surgery.launchpad.foundation.md)
- Type: [Invalid](/LaunchPad/pr-preview/pr-3327/api/rocket.surgery.launchpad.foundation/invalid.md)
- Kind: `Constructors`
- Members: `1`

The request is malformed or can never succeed as sent. HTTP 400, gRPC `InvalidArgument`, GraphQL `BAD_USER_INPUT`.

<a id="constructor"></a>
<a id="constructor-string"></a>

## Invalid(string?)

- Name: `Constructor(string?)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3327-merge.Sha.f618e95e07fca6c7011f5e648f5b4966f567c6b1/src/Foundation/Problems.cs#L42-L45)

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
