using FluentValidation.Validators;

namespace Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi;

[Experimental(Constants.ExperimentalId)]
public sealed class ComparisonPropertyRule : IPropertyRuleHandler
{
    Task IPropertyRuleHandler.HandleAsync(OpenApiValidationContext context, CancellationToken cancellationToken)
    {
        if (context is not { PropertyValidator: IComparisonValidator validator }) return Task.CompletedTask;

        if (!validator.ValueToCompare.IsNumeric()) return Task.CompletedTask;
        var valueToCompare = Convert.ToDecimal(validator.ValueToCompare);
        var schemaProperty = context.PropertySchema;

        switch (validator)
        {
            case { Comparison: Comparison.GreaterThanOrEqual }:
                {
                    schemaProperty.Minimum = valueToCompare;
                    return Task.CompletedTask;
                }

            case { Comparison: Comparison.GreaterThan }:
                {
                    schemaProperty.Minimum = valueToCompare;
                    schemaProperty.ExclusiveMinimum = true;
                    return Task.CompletedTask;
                }

            case { Comparison: Comparison.LessThanOrEqual }:
                {
                    schemaProperty.Maximum = valueToCompare;
                    return Task.CompletedTask;
                }

            case { Comparison: Comparison.LessThan }:
                {
                    schemaProperty.Maximum = valueToCompare;
                    schemaProperty.ExclusiveMaximum = true;
                    return Task.CompletedTask;
                }
        }

        return Task.CompletedTask;
    }
}