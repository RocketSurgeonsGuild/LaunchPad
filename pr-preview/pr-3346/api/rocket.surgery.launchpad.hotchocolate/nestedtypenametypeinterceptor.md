# NestedTypeNameTypeInterceptor

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3346/api/rocket.surgery.launchpad.hotchocolate.md)
- Version: `2.0.1-pr.1292`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3346-merge.Sha.ddf4e8df3db056914fd776aa443a00dc7536dd57/src/HotChocolate/NestedTypeNameTypeInterceptor.cs)
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

- [NestedTypeNameTypeInterceptor](/LaunchPad/pr-preview/pr-3346/api/rocket.surgery.launchpad.hotchocolate/nestedtypenametypeinterceptor/constructors.md#constructor)

## Methods

- [OnBeforeCompleteName(ITypeCompletionContext, TypeSystemConfiguration)](/LaunchPad/pr-preview/pr-3346/api/rocket.surgery.launchpad.hotchocolate/nestedtypenametypeinterceptor/methods.md#onbeforecompletename-itypecompletioncontext-typesystemconfiguration) -- This event is called before the type name is assigned.
