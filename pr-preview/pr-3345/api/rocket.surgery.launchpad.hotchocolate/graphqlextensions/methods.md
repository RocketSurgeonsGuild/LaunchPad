# GraphqlExtensions Methods

- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.hotchocolate.md)
- Type: [GraphqlExtensions](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.hotchocolate/graphqlextensions.md)
- Kind: `Methods`
- Members: `1`

GraphQl extensions

<a id="withproblemdetails"></a>
<a id="withproblemdetails-errorbuilder-iproblemdetailsdata"></a>

## WithProblemDetails(ErrorBuilder, IProblemDetailsData)

- Name: `WithProblemDetails(ErrorBuilder, IProblemDetailsData)`
- Modifiers: `extension`
- Returns: `ErrorBuilder`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3345-merge.Sha.a9c0325e0c8c70bc9d0569ac4df1fe5d29ffa4ec/src/HotChocolate/GraphqlExtensions.cs#L20-L32)

Add problem details to the error

```csharp
public static class GraphqlExtensions
{
    public static ErrorBuilder WithProblemDetails(
        this ErrorBuilder error,
        IProblemDetailsData data)
    {
        // ...
    }
}
```

## Parameters

- `error` (`ErrorBuilder`)
- `data` (`IProblemDetailsData`)
