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
}
