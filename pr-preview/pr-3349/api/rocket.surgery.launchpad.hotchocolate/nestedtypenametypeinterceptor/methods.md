# NestedTypeNameTypeInterceptor Methods

- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3349/api/rocket.surgery.launchpad.hotchocolate.md)
- Type: [NestedTypeNameTypeInterceptor](/LaunchPad/pr-preview/pr-3349/api/rocket.surgery.launchpad.hotchocolate/nestedtypenametypeinterceptor.md)
- Kind: `Methods`
- Members: `1`

Adds a type interceptor to be aware of nested types and ensure that they get named with the outer type and the inner type combined

<a id="onbeforecompletename"></a>
<a id="onbeforecompletename-itypecompletioncontext-typesystemconfiguration"></a>

## OnBeforeCompleteName(ITypeCompletionContext, TypeSystemConfiguration)

- Name: `OnBeforeCompleteName(ITypeCompletionContext, TypeSystemConfiguration)`
- Modifiers: `override`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3349-merge.Sha.70168524ff777a6c4fd1a4bf57611d64095a2c00/src/HotChocolate/NestedTypeNameTypeInterceptor.cs#L16-L36)

This event is called before the type name is assigned.

```csharp
public class NestedTypeNameTypeInterceptor
{
    public override void OnBeforeCompleteName(
        ITypeCompletionContext completionContext,
        TypeSystemConfiguration configuration)
    {
        // ...
    }
}
```

## Parameters

- `completionContext` (`ITypeCompletionContext`)
  The type completion context.
- `configuration` (`TypeSystemConfiguration`)
  The type system configuration of the type system member.
