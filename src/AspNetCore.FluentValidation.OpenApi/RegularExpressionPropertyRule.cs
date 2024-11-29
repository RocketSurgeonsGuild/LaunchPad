using FluentValidation.Validators;

namespace Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi;

[Experimental(Constants.ExperimentalId)]
public sealed class RegularExpressionPropertyRule : IPropertyRuleHandler
{
    Task IPropertyRuleHandler.HandleAsync(OpenApiValidationContext context, CancellationToken cancellationToken)
    {
        if (context is not { PropertyValidator: IRegularExpressionValidator validator }) return Task.CompletedTask;

        var anyPatterns = context.PropertySchema.AllOf.Any(schema => schema.Pattern is { });
        if (context.PropertySchema is { Pattern: { } } || anyPatterns)
        {
            if (!anyPatterns) context.PropertySchema.AllOf.Add(new() { Pattern = context.PropertySchema.Pattern });
            context.PropertySchema.AllOf.Add(new() { Pattern = validator.Expression });
            context.PropertySchema.Pattern = null;
        }
        else
        {
            context.PropertySchema.Pattern = validator.Expression;
        }

        return Task.CompletedTask;
    }
}