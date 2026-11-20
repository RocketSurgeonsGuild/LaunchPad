# ValidationFailureDetail Constructors

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Type: [ValidationFailureDetail](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailuredetail.md)
- Kind: `Constructors`
- Members: `1`

A single validation failure.

<a id="constructor"></a>
<a id="constructor-string-string-string"></a>

## ValidationFailureDetail(string, string, string?)

- Name: `Constructor(string, string, string?)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3307-merge.Sha.0d6c4ca8b5b3c5055adee085d785c5712bbc3314/src/ZeroAlloc.AspNetCore/Validation/ValidationFailureProblemDetails.cs#L49)

A single validation failure.

```csharp
public record ValidationFailureDetail
{
    public ValidationFailureDetail(
        string PropertyName,
        string ErrorMessage,
        string? ErrorCode)
    {
        // ...
    }
}
```

## Parameters

- `PropertyName` (`string`)
  The property that failed.
- `ErrorMessage` (`string`)
  Why it failed.
- `ErrorCode` (`string?`)
  The code of the rule that failed.
