# GraphqlExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3310/api/rocket.surgery.launchpad.hotchocolate.md)
- Version: `2.0.1-pr.1264`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3310-merge.Sha.8860010d83b5d02194514237744998b27c36d06d/src/HotChocolate/GraphqlExtensions.cs)

GraphQl extensions

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.HotChocolate;

public static class GraphqlExtensions
{
    // ...
}
```

## Methods

- [ConfigureStronglyTypedId(IRequestExecutorBuilder, Func<TStrongType, TValue>, Func<TValue, TStrongType>)](/LaunchPad/pr-preview/pr-3310/api/rocket.surgery.launchpad.hotchocolate/graphqlextensions/methods.md#configurestronglytypedid-irequestexecutorbuilder-func-tstrongtype-tvalue-func-tvalue-tstrongtype) : `IRequestExecutorBuilder` `extension` -- Configures a strongly typed id with the GraphQL scalar type of its underlying value: converts between the id and its value, binds the id to the scalar type, and filters it as that scalar.
- [WithProblemDetails(ErrorBuilder, IProblemDetailsData)](/LaunchPad/pr-preview/pr-3310/api/rocket.surgery.launchpad.hotchocolate/graphqlextensions/methods.md#withproblemdetails-errorbuilder-iproblemdetailsdata) : `ErrorBuilder` `extension` -- Add problem details to the error
