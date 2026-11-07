# NestedTypeNameTypeInterceptor

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3137/api/rocket.surgery.launchpad.hotchocolate.md)
- Version: `2.0.1-pr.1155`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate`
- Target framework: `net10.0`
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

- [NestedTypeNameTypeInterceptor](/LaunchPad/pr-preview/pr-3137/api/rocket.surgery.launchpad.hotchocolate/nestedtypenametypeinterceptor/constructors.md#constructor)

## Methods

- [OnBeforeCompleteName(ITypeCompletionContext, DefinitionBase?)](/LaunchPad/pr-preview/pr-3137/api/rocket.surgery.launchpad.hotchocolate/nestedtypenametypeinterceptor/methods.md#onbeforecompletename-itypecompletioncontext-definitionbase) -- This event is called before the type name is assigned.
