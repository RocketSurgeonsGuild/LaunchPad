# NestedTypeNameTypeInterceptor Methods

- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3137/api/rocket.surgery.launchpad.hotchocolate.md)
- Type: [NestedTypeNameTypeInterceptor](/LaunchPad/pr-preview/pr-3137/api/rocket.surgery.launchpad.hotchocolate/nestedtypenametypeinterceptor.md)
- Kind: `Methods`
- Members: `1`

Adds a type interceptor to be aware of nested types and ensure that they get named with the outer type and the inner type combined

<a id="onbeforecompletename"></a>
<a id="onbeforecompletename-itypecompletioncontext-definitionbase"></a>

## OnBeforeCompleteName(ITypeCompletionContext, DefinitionBase?)

- Name: `OnBeforeCompleteName(ITypeCompletionContext, DefinitionBase?)`
- Modifiers: `override`

This event is called before the type name is assigned.

```csharp
public class NestedTypeNameTypeInterceptor
{
    public override void OnBeforeCompleteName(
        ITypeCompletionContext completionContext,
        DefinitionBase? definition)
    {
        // ...
    }
}
```

## Parameters

- `completionContext` (`ITypeCompletionContext`)
  The type completion context.
- `definition` (`DefinitionBase?`)
  The type definition of the type system member.
