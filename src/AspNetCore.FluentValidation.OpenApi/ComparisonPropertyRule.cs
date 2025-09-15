using System.Globalization;
using FluentValidation.Validators;

namespace Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi;

[Experimental(Constants.ExperimentalId)]
public sealed class ComparisonPropertyRule : IPropertyRuleHandler
{
    Task IPropertyRuleHandler.HandleAsync(OpenApiValidationContext context, CancellationToken cancellationToken)
    {
        if (context is not { PropertyValidator: IComparisonValidator validator } || !validator.ValueToCompare.IsNumeric()) return Task.CompletedTask;

        var valueToCompare = Convert.ToDecimal(validator.ValueToCompare).ToString(CultureInfo.InvariantCulture);
        var schemaProperty = context.PropertySchema;

        switch (validator)
        {
            case { Comparison: Comparison.GreaterThanOrEqual }:
                {
                    schemaProperty.Minimum = valueToCompare;
                    break;
                }

            case { Comparison: Comparison.GreaterThan }:
                {
                    schemaProperty.Minimum = valueToCompare;
                    schemaProperty.ExclusiveMinimum = "true";
                    break;
                }

            case { Comparison: Comparison.LessThanOrEqual }:
                {
                    schemaProperty.Maximum = valueToCompare;
                    break;
                }

            case { Comparison: Comparison.LessThan }:
                {
                    schemaProperty.Maximum = valueToCompare;
                    schemaProperty.ExclusiveMaximum = "true";
                    break;
                }
        }

        return Task.CompletedTask;
    }
}
