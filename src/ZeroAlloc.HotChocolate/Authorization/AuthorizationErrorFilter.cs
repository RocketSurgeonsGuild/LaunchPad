using HotChocolate;
using HotChocolate.Execution;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Authorization;

/// <summary>
///     Reports a field HotChocolate's authorization rejected as the <c>Unauthorized</c> or <c>Forbidden</c> case of a union response is
///     reported: <c>UNAUTHENTICATED</c> or <c>FORBIDDEN</c> as the code, and the case name in <c>extensions.case</c>. A client reads one
///     shape, whether the edge or the mediator pipeline rejected the call.
/// </summary>
[PublicAPI]
public sealed class AuthorizationErrorFilter : IErrorFilter
{
    /// <summary>HotChocolate's code for a caller that is not authenticated.</summary>
    public const string NotAuthenticated = "AUTH_NOT_AUTHENTICATED";

    /// <summary>HotChocolate's code for a caller that is not allowed.</summary>
    public const string NotAuthorized = "AUTH_NOT_AUTHORIZED";

    /// <inheritdoc />
    public IError OnError(IError error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return error.Code switch
        {
            NotAuthenticated => error.WithCode("UNAUTHENTICATED").SetExtension("case", "Unauthorized"),
            NotAuthorized => error.WithCode("FORBIDDEN").SetExtension("case", "Forbidden"),
            _ => error,
        };
    }
}
