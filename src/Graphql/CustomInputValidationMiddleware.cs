﻿using FairyBread;
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
        private readonly IValidationErrorsHandler _validationErrorsHandler;

        public CustomInputValidationMiddleware(
            FieldDelegate next,
            IFairyBreadOptions options,
            IValidatorProvider validatorProvider,
            IValidationErrorsHandler validationErrorsHandler
        )
        {
            _next = next;
            _options = options;
            _validatorProvider = validatorProvider;
            _validationErrorsHandler = validationErrorsHandler;
        }


        public async Task InvokeAsync(IMiddlewareContext context)
        {
            var arguments = context.Field.Arguments;

            var invalidResults = new List<ValidationResult>();

            foreach (var argument in arguments)
            {
                if (argument == null ||
                    !_options.ShouldValidate(context, argument))
                {
                    continue;
                }

                var resolvedValidators = _validatorProvider.GetValidators(context, argument);
                try
                {
                    var value = context.ArgumentValue<object?>(argument.Name);
                    if (value == null)
                    {
                        continue;
                    }

                    foreach (var resolvedValidator in resolvedValidators)
                    {
                        var validationContext = new ValidationContext<object?>(value);
                        validationContext.SetServiceProvider(context.Services);
                        var validationResult = await resolvedValidator.Validator.ValidateAsync(
                            validationContext,
                            context.RequestAborted
                        );
                        if (validationResult != null &&
                            !validationResult.IsValid)
                        {
                            invalidResults.Add(validationResult);
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

            if (invalidResults.Any())
            {
                _validationErrorsHandler.Handle(context, invalidResults);
                context.Result = null;
            }
            else
            {
                await _next(context);
            }
        }
    }
}