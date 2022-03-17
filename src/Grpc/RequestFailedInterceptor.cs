using Grpc.Core;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Rocket.Surgery.LaunchPad.Grpc;

internal class RequestFailedInterceptor : ProblemDetailsInterceptor<RequestFailedException>
{
    public RequestFailedInterceptor() : base(StatusCode.FailedPrecondition)
    {
    }
}
