# ValidationFailureDetail Constructors

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Type: [ValidationFailureDetail](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailuredetail.md)
- Kind: `Constructors`
- Members: `1`

A single validation failure.

<a id="constructor"></a>
<a id="constructor-string-string-string"></a>

## ValidationFailureDetail(string, string, string?)

- Name: `Constructor(string, string, string?)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3323-merge.Sha.d852442c68f31f012f495e373ebac8fda5751397/src/ZeroAlloc.AspNetCore/Validation/ValidationFailureProblemDetails.cs#L49)

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
