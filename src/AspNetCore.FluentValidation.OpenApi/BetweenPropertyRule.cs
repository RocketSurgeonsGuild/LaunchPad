using FluentValidation.Validators;

namespace Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi;

[Experimental(Constants.ExperimentalId)]
public sealed class BetweenPropertyRule : IPropertyRuleHandler
{
    Task IPropertyRuleHandler.HandleAsync(OpenApiValidationContext context, CancellationToken cancellationToken)
    {
        if (context is not { PropertyValidator: IBetweenValidator validator }) return Task.CompletedTask;

        var schemaProperty = context.PropertySchema;
        if (validator.From.IsNumeric())
        {
            schemaProperty.Minimum = Convert.ToDecimal(validator.From);
            if (validator.Name == "ExclusiveBetweenValidator") schemaProperty.ExclusiveMinimum = true;
        }

        if (validator.To.IsNumeric())
        {
            schemaProperty.Maximum = Convert.ToDecimal(validator.To);
            if (validator.Name == "ExclusiveBetweenValidator") schemaProperty.ExclusiveMaximum = true;
        }

        return Task.CompletedTask;
    }
}