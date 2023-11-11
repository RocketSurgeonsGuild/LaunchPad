using Grpc.Core;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Rocket.Surgery.LaunchPad.Grpc;

internal class NotFoundInterceptor() : ProblemDetailsInterceptor<NotFoundException>(StatusCode.NotFound);
