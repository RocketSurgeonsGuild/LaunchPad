using FluentValidation;
using FluentValidation.Results;
using HotChocolate;
using Rocket.Surgery.LaunchPad.AspNetCore.Validation;
using Rocket.Surgery.LaunchPad.Foundation;
using System.Collections.ObjectModel;
using System.Linq;

namespace Rocket.Surgery.LaunchPad.Graphql
{
    class GraphqlErrorFilter : IErrorFilter
    {
        public IError OnError(IError error)
        {
            if (error.Exception is IProblemDetailsData ex)
            {
                return ErrorBuilder.FromError(error)
                   .SetMessage(error.Exception.Message)
                   .RemoveException()
                   .WithProblemDetails(ex)
                   .Build();
            }

            if (error.Exception is ValidationException vx)
            {
                return ErrorBuilder.FromError(error)
                   .SetMessage(vx.Message)
                   .RemoveException()
                   .SetExtension("type", "ValidationProblemDetails")
                   .SetExtension("title", "Unprocessable Entity")
                   .SetExtension("link", "https://tools.ietf.org/html/rfc4918#section-11.2")
                   .SetExtension("errors", vx.Errors.Select<ValidationFailure, object>(FormatFailure).ToArray())
                   .Build();
                // return ErrorBuilder.FromError(error)
                //    .SetMessage(vx.Message)
                //    .SetException(null)
                //    .Build();
            }

            return error;
        }

        protected virtual ReadOnlyDictionary<string, object?> FormatFailure(ValidationFailure failure) => new FluentValidationProblemDetail(failure);
    }
}