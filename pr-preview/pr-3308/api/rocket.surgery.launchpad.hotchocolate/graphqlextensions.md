# GraphqlExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3308/api/rocket.surgery.launchpad.hotchocolate.md)
- Version: `2.0.1-pr.1261`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3308-merge.Sha.b5321c7138bef72baa9f203b3f824fa7207c9dcf/src/HotChocolate/GraphqlExtensions.cs)

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

- [ConfigureStronglyTypedId(IRequestExecutorBuilder, Func<TStrongType, TValue>, Func<TValue, TStrongType>)](/LaunchPad/pr-preview/pr-3308/api/rocket.surgery.launchpad.hotchocolate/graphqlextensions/methods.md#configurestronglytypedid-irequestexecutorbuilder-func-tstrongtype-tvalue-func-tvalue-tstrongtype) : `IRequestExecutorBuilder` `extension` -- Configures a strongly typed id with the GraphQL scalar type of its underlying value: converts between the id and its value, binds the id to the scalar type, and filters it as that scalar.
- [WithProblemDetails(ErrorBuilder, IProblemDetailsData)](/LaunchPad/pr-preview/pr-3308/api/rocket.surgery.launchpad.hotchocolate/graphqlextensions/methods.md#withproblemdetails-errorbuilder-iproblemdetailsdata) : `ErrorBuilder` `extension` -- Add problem details to the error
