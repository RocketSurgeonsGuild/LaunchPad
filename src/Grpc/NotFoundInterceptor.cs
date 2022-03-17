using Grpc.Core;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Rocket.Surgery.LaunchPad.Grpc;

internal class NotFoundInterceptor : ProblemDetailsInterceptor<NotFoundException>
{
    public NotFoundInterceptor() : base(StatusCode.NotFound)
    {
    }
}
