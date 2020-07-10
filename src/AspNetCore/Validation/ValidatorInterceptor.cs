using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Validation
{
    internal class ValidatorInterceptor : IValidatorInterceptor
    {
        public ValidationContext BeforeMvcValidation(
            ControllerContext controllerContext,
            ValidationContext validationContext
        ) => validationContext;

        public ValidationResult AfterMvcValidation(
            ControllerContext controllerContext,
            ValidationContext validationContext,
            ValidationResult result
        )
        {
            controllerContext.HttpContext.Items[typeof(ValidationResult)] = result;
            return result;
        }
    }
}