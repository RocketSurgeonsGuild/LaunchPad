using FluentValidation.Validators;

namespace Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi;

[Experimental(Constants.ExperimentalId)]
public sealed class RequiredPropertyRuleHandler : IPropertyRuleHandler
{
    Task IPropertyRuleHandler.HandleAsync(OpenApiValidationContext context, CancellationToken cancellationToken)
    {
        if (context.PropertyValidator is not (INotNullValidator or INotEmptyValidator)) return Task.CompletedTask;
        context.TypeSchema.Required.Add(context.PropertyKey);
        return Task.CompletedTask;
    }
}
