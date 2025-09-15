using FluentValidation.Validators;
using Microsoft.OpenApi;

namespace Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi;

[Experimental(Constants.ExperimentalId)]
public sealed class RegularExpressionPropertyRule : IPropertyRuleHandler
{
    Task IPropertyRuleHandler.HandleAsync(OpenApiValidationContext context, CancellationToken cancellationToken)
    {
        if (context is not { PropertyValidator: IRegularExpressionValidator validator }) return Task.CompletedTask;

        context.PropertySchema.AllOf ??= new List<IOpenApiSchema>();
        var anyPatterns = context.PropertySchema.AllOf.Any(schema => schema.Pattern is { });
        if (context.PropertySchema is { Pattern: { } } || anyPatterns)
        {
            if (!anyPatterns) context.PropertySchema.AllOf.Add(new OpenApiSchema { Pattern = context.PropertySchema.Pattern });
            context.PropertySchema.AllOf.Add(new OpenApiSchema() { Pattern = validator.Expression });
            context.PropertySchema.Pattern = null;
        }
        else
        {
            context.PropertySchema.Pattern = validator.Expression;
        }

        return Task.CompletedTask;
    }
}
