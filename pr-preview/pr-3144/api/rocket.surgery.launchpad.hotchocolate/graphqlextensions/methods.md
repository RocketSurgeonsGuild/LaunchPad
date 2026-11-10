# GraphqlExtensions Methods

- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3144/api/rocket.surgery.launchpad.hotchocolate.md)
- Type: [GraphqlExtensions](/LaunchPad/pr-preview/pr-3144/api/rocket.surgery.launchpad.hotchocolate/graphqlextensions.md)
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
<a id="withproblemdetails-ierrorbuilder-iproblemdetailsdata"></a>

## WithProblemDetails(IErrorBuilder, IProblemDetailsData)

- Name: `WithProblemDetails(IErrorBuilder, IProblemDetailsData)`
- Modifiers: `extension`
- Returns: `IErrorBuilder`

Add problem details to the error

```csharp
public static class GraphqlExtensions
{
    public static IErrorBuilder WithProblemDetails(
        this IErrorBuilder error,
        IProblemDetailsData data)
    {
        // ...
    }
}
```

## Parameters

- `error` (`IErrorBuilder`)
- `data` (`IProblemDetailsData`)
