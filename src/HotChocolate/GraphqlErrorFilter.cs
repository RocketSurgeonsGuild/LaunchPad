using HotChocolate;
using HotChocolate.Execution;

namespace Rocket.Surgery.LaunchPad.HotChocolate;

internal class GraphqlErrorFilter : IErrorFilter
{
    /*
     protected virtual IErrorBuilder FormatFailure(ValidationFailure failure)

    {
        var builder = ErrorBuilder
                     .New()
                     .SetMessage(failure.ErrorMessage)
                     .SetCode(failure.ErrorCode)
                     .SetExtension("errorCode", failure.ErrorCode)
                     .SetExtension("errorMessage", failure.ErrorMessage)
                     .SetExtension("attemptedValue", failure.AttemptedValue)
                     .SetExtension("severity", failure.Severity);

        if (!string.IsNullOrWhiteSpace(failure.PropertyName))
        {
            builder = builder
                     .SetExtension("field", failure.PropertyName)
                     .SetExtension("propertyName", failure.PropertyName);
        }

        return builder;
    }
    */

    public IError OnError(IError error)
    {
        /*
        if (error.Exception is ValidationException vx)
        {
            var childErrors =
                vx
                   .Errors
                   .Select(failure => FormatFailure(failure).Build());
            error = new AggregateError(childErrors);
            error.WithCode("VALIDATION");
        }
        */

        var builder = ErrorBuilder.FromError(error);

        if (error.Exception is { })
        {
            builder
               .SetException(error.Exception)
               .SetMessage(error.Exception?.Message ?? error.Message)
               .SetCode(error.Code ?? "UNKNOWN");
        }

        return builder.Build();
    }
}
