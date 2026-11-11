# UnprocessableEntity Constructors

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3191/api/rocket.surgery.launchpad.foundation.md)
- Type: [UnprocessableEntity](/LaunchPad/pr-preview/pr-3191/api/rocket.surgery.launchpad.foundation/unprocessableentity.md)
- Kind: `Constructors`
- Members: `1`

The request is well formed but fails validation: what FluentValidation reports. HTTP 422, gRPC `InvalidArgument`, GraphQL `BAD_USER_INPUT`.

<a id="constructor"></a>
<a id="constructor-string"></a>

## UnprocessableEntity(string?)

- Name: `Constructor(string?)`

The request is well formed but fails validation: what FluentValidation reports. HTTP 422, gRPC `InvalidArgument`, GraphQL `BAD_USER_INPUT`.

```csharp
public record UnprocessableEntity
{
    public UnprocessableEntity(
        string? Detail = null)
    {
        // ...
    }
}
```

## Parameters

- `Detail` (`string?`) `optional`
  A human-readable explanation specific to this occurrence of the problem.
