using FluentValidation.Validators;

namespace Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi;

[Experimental(Constants.ExperimentalId)]
public sealed class NotEmptyPropertyRule : IPropertyRuleHandler
{
    Task IPropertyRuleHandler.HandleAsync(OpenApiValidationContext context, CancellationToken cancellationToken)
    {
        if (context is { PropertyValidator: not INotEmptyValidator } or { PropertySchema.MinLength: > 1 } or { PropertySchema.Type: not ("string" or "array") })
            return Task.CompletedTask;
        context.PropertySchema.MinLength = 1;
        return Task.CompletedTask;
    }
}