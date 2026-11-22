# NestedTypeNameTypeInterceptor

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3334/api/rocket.surgery.launchpad.hotchocolate.md)
- Version: `2.0.1-pr.1284`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3334-merge.Sha.1bdca6582f82d4de4d3840709c4a6fa63d4c5f0c/src/HotChocolate/NestedTypeNameTypeInterceptor.cs)
- Inherits: `TypeInterceptor`

Adds a type interceptor to be aware of nested types and ensure that they get named with the outer type and the inner type combined

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.HotChocolate;

public class NestedTypeNameTypeInterceptor
    : HotChocolate.Configuration.TypeInterceptor
{
    // ...
}
```

## Constructors

- [NestedTypeNameTypeInterceptor](/LaunchPad/pr-preview/pr-3334/api/rocket.surgery.launchpad.hotchocolate/nestedtypenametypeinterceptor/constructors.md#constructor)

## Methods

- [OnBeforeCompleteName(ITypeCompletionContext, TypeSystemConfiguration)](/LaunchPad/pr-preview/pr-3334/api/rocket.surgery.launchpad.hotchocolate/nestedtypenametypeinterceptor/methods.md#onbeforecompletename-itypecompletioncontext-typesystemconfiguration) -- This event is called before the type name is assigned.
