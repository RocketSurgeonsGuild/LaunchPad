using FluentValidation.Validators;

namespace Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi;

[Experimental(Constants.ExperimentalId)]
public sealed class LengthPropertyRule : IPropertyRuleHandler
{
    Task IPropertyRuleHandler.HandleAsync(OpenApiValidationContext context, CancellationToken cancellationToken)
    {
        if (context.PropertyValidator is not ILengthValidator validator) return Task.CompletedTask;

        if (context.PropertySchema.Type == "array")
        {
            if (validator.Max > 0)
                context.PropertySchema.MaxItems = validator.Max;

            if (validator.Min > 0)
                context.PropertySchema.MinItems = validator.Min;
        }
        else
        {
            if (validator.Max > 0)
                context.PropertySchema.MaxLength = validator.Max;
            if (validator.Min > 0)
                context.PropertySchema.MinLength = validator.Min;
        }

        return Task.CompletedTask;
    }
}