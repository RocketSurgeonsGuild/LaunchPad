# NestedTypeNameTypeInterceptor Methods

- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3351/api/rocket.surgery.launchpad.hotchocolate.md)
- Type: [NestedTypeNameTypeInterceptor](/LaunchPad/pr-preview/pr-3351/api/rocket.surgery.launchpad.hotchocolate/nestedtypenametypeinterceptor.md)
- Kind: `Methods`
- Members: `1`

Adds a type interceptor to be aware of nested types and ensure that they get named with the outer type and the inner type combined

<a id="onbeforecompletename"></a>
<a id="onbeforecompletename-itypecompletioncontext-typesystemconfiguration"></a>

## OnBeforeCompleteName(ITypeCompletionContext, TypeSystemConfiguration)

- Name: `OnBeforeCompleteName(ITypeCompletionContext, TypeSystemConfiguration)`
- Modifiers: `override`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3351-merge.Sha.a0ae38aa18a1b9917789ec429da9c9c80b65406f/src/HotChocolate/NestedTypeNameTypeInterceptor.cs#L16-L36)

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
