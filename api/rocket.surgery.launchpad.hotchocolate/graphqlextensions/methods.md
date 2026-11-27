# GraphqlExtensions Methods

- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/api/rocket.surgery.launchpad.hotchocolate.md)
- Type: [GraphqlExtensions](/LaunchPad/api/rocket.surgery.launchpad.hotchocolate/graphqlextensions.md)
- Kind: `Methods`
- Members: `1`

GraphQl extensions

<a id="withproblemdetails"></a>
<a id="withproblemdetails-errorbuilder-iproblemdetailsdata"></a>

## WithProblemDetails(ErrorBuilder, IProblemDetailsData)

- Name: `WithProblemDetails(ErrorBuilder, IProblemDetailsData)`
- Modifiers: `extension`
- Returns: `ErrorBuilder`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.cd7db14e49b12478b13b2f21edf214c613078b4c/src/HotChocolate/GraphqlExtensions.cs#L20-L32)

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
