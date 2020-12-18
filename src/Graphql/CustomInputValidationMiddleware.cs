using FairyBread;
using FluentValidation;
using FluentValidation.Results;
using HotChocolate.Resolvers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocket.Surgery.LaunchPad.Graphql
{
    /// <summary>
    /// This is just to ensure the service provider is set.
    /// </summary>
    class CustomInputValidationMiddleware
    {
        private readonly FieldDelegate _next;
        private readonly IFairyBreadOptions _options;
        private readonly IValidatorProvider _validatorProvider;
        private readonly IValidationResultHandler _validationResultHandler;

        public CustomInputValidationMiddleware(
            FieldDelegate next,
            IFairyBreadOptions options,
            IValidatorProvider validatorProvider,
            IValidationResultHandler validationResultHandler
        )
        {
            _next = next;
            _options = options;
            _validatorProvider = validatorProvider;
            _validationResultHandler = validationResultHandler;
        }

        public async Task InvokeAsync(IMiddlewareContext context)
        {
            var arguments = context.Field.Arguments;

            var validationResults = new List<ValidationResult>();

            foreach (var argument in arguments)
            {
                if (argument == null ||
                    !_options.ShouldValidate(context, argument))
                {
                    continue;
                }

                var resolvedValidators = _validatorProvider.GetValidators(context, argument).ToArray();
                try
                {
                    var value = context.ArgumentValue<object>(argument.Name);
                    foreach (var resolvedValidator in resolvedValidators)
                    {
                        var validationContext = new ValidationContext<object>(value);
                        validationContext.SetServiceProvider(context.Services);
                        var validationResult = await resolvedValidator.Validator.ValidateAsync(validationContext, context.RequestAborted);
                        if (validationResult != null)
                        {
                            validationResults.Add(validationResult);
                            _validationResultHandler.Handle(context, validationResult);
                        }
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

            var invalidValidationResults = validationResults.Where(r => !r.IsValid);
            if (invalidValidationResults.Any())
            {
                OnInvalid(context, invalidValidationResults);
            }

            await _next(context);
        }

        protected virtual void OnInvalid(IMiddlewareContext context, IEnumerable<ValidationResult> invalidValidationResults)
        {
            throw new ValidationException(invalidValidationResults.SelectMany(vr => vr.Errors));
        }
    }
}