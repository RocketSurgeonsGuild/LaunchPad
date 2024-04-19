using FairyBread;
using FluentValidation;
using HotChocolate.Resolvers;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Validation;

[UsedImplicitly]
internal class ValidationMiddleware
(
    FieldDelegate next,
    IValidatorProvider validatorProvider,
    IValidationErrorsHandler validationErrorsHandler
)
{
    public async Task InvokeAsync(IMiddlewareContext context)
    {
        var arguments = context.Selection.Field.Arguments;

        var invalidResults = new List<ArgumentValidationResult>();

        foreach (var argument in arguments)
        {
            var resolvedValidators = validatorProvider
                                    .GetValidators(context, argument)
                                    .ToArray();
            if (resolvedValidators.Length > 0)
                try
                {
                    var value = context.ArgumentValue<object?>(argument.Name);
                    if (value == null) continue;

                    foreach (var resolvedValidator in resolvedValidators)
                    {
                        var validationContext = new ValidationContext<object?>(value);
                        var validationResult = await resolvedValidator.Validator.ValidateAsync(
                            validationContext,
                            context.RequestAborted
                        );
                        if (validationResult is { IsValid: false, })
                            invalidResults.Add(
                                new(
                                    argument.Name,
                                    resolvedValidator.Validator,
                                    validationResult
                                )
                            );
                    }
                }
                finally
                {
                    foreach (var resolvedValidator in resolvedValidators)
                    {
                        resolvedValidator.Scope?.Dispose();
                    }
                }
        }

        if (invalidResults.Any())
        {
            validationErrorsHandler.Handle(context, invalidResults);
            return;
        }

        await next(context);
    }
}