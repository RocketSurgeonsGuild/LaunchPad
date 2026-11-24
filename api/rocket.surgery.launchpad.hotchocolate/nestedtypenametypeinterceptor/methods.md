# NestedTypeNameTypeInterceptor Methods

- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/api/rocket.surgery.launchpad.hotchocolate.md)
- Type: [NestedTypeNameTypeInterceptor](/LaunchPad/api/rocket.surgery.launchpad.hotchocolate/nestedtypenametypeinterceptor.md)
- Kind: `Methods`
- Members: `1`

Adds a type interceptor to be aware of nested types and ensure that they get named with the outer type and the inner type combined

<a id="onbeforecompletename"></a>
<a id="onbeforecompletename-itypecompletioncontext-typesystemconfiguration"></a>

## OnBeforeCompleteName(ITypeCompletionContext, TypeSystemConfiguration)

- Name: `OnBeforeCompleteName(ITypeCompletionContext, TypeSystemConfiguration)`
- Modifiers: `override`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.4a5cb286621c756019f759ab46381031aa177050/src/HotChocolate/NestedTypeNameTypeInterceptor.cs#L16-L36)

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
