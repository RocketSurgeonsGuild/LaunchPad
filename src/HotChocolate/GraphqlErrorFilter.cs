using System.Collections.ObjectModel;
using FluentValidation;
using FluentValidation.Results;
using HotChocolate;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Rocket.Surgery.LaunchPad.HotChocolate;

internal class GraphqlErrorFilter : IErrorFilter
{
    protected virtual ReadOnlyDictionary<string, object?> FormatFailure(ValidationFailure failure)
    {
        return new FluentValidationProblemDetail(failure);
    }

    public IError OnError(IError error)
    {
        if (error.Exception is ValidationException vx)
        {
            var childErrors =
                vx.Errors.Select(x => new FluentValidationProblemDetail(x))
                  .Select(
                       x => new Error(x.ErrorMessage)
                           .WithCode(x.ErrorCode)
                           .SetExtension("severity", x.Severity.ToString())
                           .SetExtension("attemptedValue", x.AttemptedValue)
                           .SetExtension("field", x.PropertyName)
                           .SetExtension("propertyName", x.PropertyName)
                   );
            var result = new AggregateError(childErrors);
            return result;
        }

        if (error.Exception is IProblemDetailsData ex)
        {
            var builder = ErrorBuilder.FromError(error);
            builder
               .SetException(error.Exception)
               .SetMessage(error.Exception.Message)
               .WithProblemDetails(ex);

            if (error.Exception is NotFoundException)
            {
                builder.SetCode("NOTFOUND");
            }

            if (error.Exception is RequestFailedException)
            {
                builder.SetCode("FAILED");
            }

            return builder.Build();
        }

        return error;
    }
}
