using HotChocolate;
using HotChocolate.Execution;

using ZeroAlloc.Mediator.Validation;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Validation;

/// <summary>
///     Reports each failure of a request the ZeroAlloc validators rejected as a <c>VALIDATION</c> error, with the property name and the error
///     code as extensions.
/// </summary>
[PublicAPI]
public sealed class ZeroAllocValidationErrorFilter : IErrorFilter
{
    /// <summary>The error code of a validation failure.</summary>
    public const string ValidationCode = "VALIDATION";

    /// <inheritdoc />
    public IError OnError(IError error)
    {
        ArgumentNullException.ThrowIfNull(error);
        if (error.Exception is not ValidationFailedException exception) return error;

        var errors = exception
                    .Error.Failures
                    .ToArray()
                    .Select(
                         failure => error
                                   .WithException(null)
                                   .RemoveExtension("message")
                                   .RemoveExtension("stackTrace")
                                   .WithMessage(failure.ErrorMessage)
                                   .WithCode(ValidationCode)
                                   .SetExtension("propertyName", failure.PropertyName)
                                   .SetExtension("errorCode", failure.ErrorCode)
                     )
                    .ToArray();
        return errors.Length == 1 ? errors[0] : new AggregateError(errors);
    }
}
