using Grpc.Core;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Rocket.Surgery.LaunchPad.Primitives;

namespace Rocket.Surgery.LaunchPad.Grpc;

internal class RequestFailedInterceptor(IOptions<JsonOptions> options) : ProblemDetailsInterceptor<RequestFailedException>(StatusCode.FailedPrecondition, options.Value.SerializerOptions);
