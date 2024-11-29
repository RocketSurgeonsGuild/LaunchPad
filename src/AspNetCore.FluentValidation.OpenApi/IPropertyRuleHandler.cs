namespace Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi;

[Experimental(Constants.ExperimentalId)]
public interface IPropertyRuleHandler
{
    Task HandleAsync(OpenApiValidationContext context, CancellationToken cancellationToken);
}