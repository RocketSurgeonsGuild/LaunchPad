# GraphqlExtensions Methods

- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.hotchocolate.md)
- Type: [GraphqlExtensions](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.hotchocolate/graphqlextensions.md)
- Kind: `Methods`
- Members: `2`

GraphQl extensions

<a id="configurestronglytypedid"></a>
<a id="configurestronglytypedid-irequestexecutorbuilder-func-tstrongtype-tvalue-func-tvalue-tstrongtype"></a>

## ConfigureStronglyTypedId(IRequestExecutorBuilder, Func<TStrongType, TValue>, Func<TValue, TStrongType>)

- Name: `ConfigureStronglyTypedId(IRequestExecutorBuilder, Func<TStrongType, TValue>, Func<TValue, TStrongType>)`
- Modifiers: `extension`
- Returns: `IRequestExecutorBuilder`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-2602-merge.Sha.f387c871fa01cb1c7bfc78f0117912dc30d931f2/src/HotChocolate/GraphqlExtensions.cs#L62-L69)

Configures a strongly typed id with the GraphQL scalar type of its underlying value: converts between the id and its value, binds the id to the scalar type, and filters it as that scalar.

```csharp
public static class GraphqlExtensions
{
    public static IRequestExecutorBuilder ConfigureStronglyTypedId<TStrongType, TValue, TSchemaType>(
        this IRequestExecutorBuilder builder,
        Func<TStrongType, TValue> toValue,
        Func<TValue, TStrongType> fromValue)
    {
        // ...
    }
}
```

## Parameters

- `builder` (`IRequestExecutorBuilder`)
  The request executor builder.
- `toValue` (`Func<TStrongType, TValue>`)
  Gets the underlying value of an id.
- `fromValue` (`Func<TValue, TStrongType>`)
  Creates an id from its underlying value.

## Returns

`IRequestExecutorBuilder` -- The builder.

## Remarks

The conversions are given rather than found by reflection, so this is trim and native AOT safe: `builder.ConfigureStronglyTypedId<RocketId, Guid, UuidType>(id => id.Value, value => new(value))`.

<a id="withproblemdetails"></a>
<a id="withproblemdetails-errorbuilder-iproblemdetailsdata"></a>

## WithProblemDetails(ErrorBuilder, IProblemDetailsData)

- Name: `WithProblemDetails(ErrorBuilder, IProblemDetailsData)`
- Modifiers: `extension`
- Returns: `ErrorBuilder`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-2602-merge.Sha.f387c871fa01cb1c7bfc78f0117912dc30d931f2/src/HotChocolate/GraphqlExtensions.cs#L25-L37)

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
