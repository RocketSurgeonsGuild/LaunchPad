# IPropertyRuleHandler Methods

- Package: [Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore](/LaunchPad/pr-preview/pr-3141/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore.md)
- Type: [IPropertyRuleHandler](/LaunchPad/pr-preview/pr-3141/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/ipropertyrulehandler.md)
- Kind: `Methods`
- Members: `1`

<a id="handleasync"></a>
<a id="handleasync-openapivalidationcontext-cancellationtoken"></a>

## HandleAsync(OpenApiValidationContext, CancellationToken)

- Name: `HandleAsync(OpenApiValidationContext, CancellationToken)`
- Modifiers: `abstract`
- Returns: `Task`

```csharp
public interface IPropertyRuleHandler
{
    public abstract Task HandleAsync(
        OpenApiValidationContext context,
        CancellationToken cancellationToken)
    {
        // ...
    }
}
```

## Parameters

- `context` ([OpenApiValidationContext](/LaunchPad/pr-preview/pr-3141/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/openapivalidationcontext.md))
- `cancellationToken` (`CancellationToken`)
