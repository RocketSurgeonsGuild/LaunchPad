# GraphqlExtensions Methods

- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3206/api/rocket.surgery.launchpad.hotchocolate.md)
- Type: [GraphqlExtensions](/LaunchPad/pr-preview/pr-3206/api/rocket.surgery.launchpad.hotchocolate/graphqlextensions.md)
- Kind: `Methods`
- Members: `2`

GraphQl extensions

<a id="configurestronglytypedid"></a>
<a id="configurestronglytypedid-irequestexecutorbuilder"></a>

## ConfigureStronglyTypedId(IRequestExecutorBuilder)

- Name: `ConfigureStronglyTypedId(IRequestExecutorBuilder)`
- Modifiers: `extension`
- Returns: `IRequestExecutorBuilder`

Configures a generated strongly typed id type with the given graphql schema type

```csharp
public static class GraphqlExtensions
{
    public static IRequestExecutorBuilder ConfigureStronglyTypedId<TStrongType, TSchemaType>(
        this IRequestExecutorBuilder builder)
    {
        // ...
    }
}
```

## Parameters

- `builder` (`IRequestExecutorBuilder`)

## Remarks

Adds converters, binding and filters.

<a id="withproblemdetails"></a>
<a id="withproblemdetails-errorbuilder-iproblemdetailsdata"></a>

## WithProblemDetails(ErrorBuilder, IProblemDetailsData)

- Name: `WithProblemDetails(ErrorBuilder, IProblemDetailsData)`
- Modifiers: `extension`
- Returns: `ErrorBuilder`

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
