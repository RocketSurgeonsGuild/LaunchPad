using Grpc.Core;

using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.Primitives;

namespace Rocket.Surgery.LaunchPad.Grpc;

internal class NotFoundInterceptor(IOptions<JsonOptions> options) : ProblemDetailsInterceptor<NotFoundException>(StatusCode.NotFound, options.Value.SerializerOptions);
