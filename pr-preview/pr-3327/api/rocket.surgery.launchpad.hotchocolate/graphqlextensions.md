# GraphqlExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3327/api/rocket.surgery.launchpad.hotchocolate.md)
- Version: `2.0.1-pr.1278`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3327-merge.Sha.f618e95e07fca6c7011f5e648f5b4966f567c6b1/src/HotChocolate/GraphqlExtensions.cs)

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

- [ConfigureStronglyTypedId(IRequestExecutorBuilder, Func<TStrongType, TValue>, Func<TValue, TStrongType>)](/LaunchPad/pr-preview/pr-3327/api/rocket.surgery.launchpad.hotchocolate/graphqlextensions/methods.md#configurestronglytypedid-irequestexecutorbuilder-func-tstrongtype-tvalue-func-tvalue-tstrongtype) : `IRequestExecutorBuilder` `extension` -- Configures a strongly typed id with the GraphQL scalar type of its underlying value: converts between the id and its value, binds the id to the scalar type, and filters it as that scalar.
- [WithProblemDetails(ErrorBuilder, IProblemDetailsData)](/LaunchPad/pr-preview/pr-3327/api/rocket.surgery.launchpad.hotchocolate/graphqlextensions/methods.md#withproblemdetails-errorbuilder-iproblemdetailsdata) : `ErrorBuilder` `extension` -- Add problem details to the error
