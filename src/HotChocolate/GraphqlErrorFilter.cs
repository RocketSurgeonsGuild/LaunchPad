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
                vx
                   .Errors.Select(x => new FluentValidationProblemDetail(x))
                   .Select(
                        failure =>
                        {
                            var err = new Error(failure.ErrorMessage)
                                     .WithCode(failure.ErrorCode)
                                     .SetExtension("attemptedValue", failure.AttemptedValue)
                                     .SetExtension("severity", failure.Severity);

                            if (!string.IsNullOrWhiteSpace(failure.PropertyName))
                                err = err
                                     .SetExtension("field", failure.PropertyName)
                                     .SetExtension("propertyName", failure.PropertyName);

                            return err;
                        }
                    );
            return new AggregateError(childErrors);
        }

        if (error.Exception is IProblemDetailsData ex)
        {
            var builder = ErrorBuilder.FromError(error);
            builder
               .SetException(error.Exception)
               .SetMessage(error.Exception.Message)
               .WithProblemDetails(ex);

            if (error.Exception is NotFoundException) builder.SetCode("NOTFOUND");

            if (error.Exception is NotAuthorizedException) builder.SetCode("NOTAUTHORIZED");

            if (error.Exception is RequestFailedException) builder.SetCode("FAILED");

            return builder.Build();
        }

        return error;
    }
}