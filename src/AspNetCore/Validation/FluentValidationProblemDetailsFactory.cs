using System;
using System.Diagnostics;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Validation
{
    internal sealed class FluentValidationProblemDetailsFactory : ProblemDetailsFactory
    {
        private readonly ApiBehaviorOptions _apiBehaviorOptions;

        public FluentValidationProblemDetailsFactory(IOptions<ApiBehaviorOptions> apiBehavior) => _apiBehaviorOptions =
            apiBehavior?.Value ?? throw new ArgumentNullException(nameof(apiBehavior));

        /// <inheritdoc />
        public override ProblemDetails CreateProblemDetails(
            HttpContext httpContext,
            int? statusCode = null,
            string? title = null,
            string? type = null,
            string? detail = null,
            string? instance = null
        )
        {
            statusCode ??= 500;

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Type = type,
                Detail = detail,
                Instance = instance
            };

            ApplyProblemDetailsDefaults(httpContext, problemDetails, statusCode.Value);

            return problemDetails;
        }

        /// <inheritdoc />
        public override ValidationProblemDetails CreateValidationProblemDetails(
            HttpContext httpContext,
            ModelStateDictionary modelStateDictionary,
            int? statusCode = null,
            string? title = null,
            string? type = null,
            string? detail = null,
            string? instance = null
        )
        {
            if (modelStateDictionary == null)
            {
                throw new ArgumentNullException(nameof(modelStateDictionary));
            }

            statusCode ??= 400;

            ValidationProblemDetails? problemDetails = null;

            if (httpContext.Items[typeof(ValidationResult)] is ValidationResult result)
            {
                statusCode = 422;
                problemDetails = new FluentValidationProblemDetails(result!)
                {
                    Status = 422,
                    Type = type,
                    Detail = detail,
                    Instance = instance
                };
            }
            else if (httpContext.Items[typeof(ValidationException)] is ValidationException failures)
            {
                statusCode = 422;
                problemDetails = new FluentValidationProblemDetails(failures.Errors)
                {
                    Status = 422,
                    Type = type,
                    Detail = detail,
                    Instance = instance
                };
            }

            if (problemDetails == null)
            {
                problemDetails = new ValidationProblemDetails(modelStateDictionary)
                {
                    Status = statusCode,
                    Type = type,
                    Detail = detail,
                    Instance = instance
                };
            }

            if (title != null)
            {
                // For validation problem details, don't overwrite the default title with null.
                problemDetails.Title = title;
            }

            ApplyProblemDetailsDefaults(httpContext, problemDetails, statusCode.Value);

            return problemDetails;
        }

        private void ApplyProblemDetailsDefaults(HttpContext httpContext, ProblemDetails problemDetails, int statusCode)
        {
            problemDetails.Status ??= statusCode;

            if (_apiBehaviorOptions.ClientErrorMapping.TryGetValue(statusCode, out var clientErrorData))
            {
                problemDetails.Title ??= clientErrorData.Title;
                problemDetails.Type ??= clientErrorData.Link;
            }

            var traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier;
            if (traceId != null)
            {
                problemDetails.Extensions["traceId"] = traceId;
            }
        }
    }
}