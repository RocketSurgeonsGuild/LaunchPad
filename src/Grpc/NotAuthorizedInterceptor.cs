using Grpc.Core;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Rocket.Surgery.LaunchPad.Grpc;

internal class NotAuthorizedInterceptor : ProblemDetailsInterceptor<NotAuthorizedException>
{
    public NotAuthorizedInterceptor() : base(StatusCode.PermissionDenied)
    {
    }
}
