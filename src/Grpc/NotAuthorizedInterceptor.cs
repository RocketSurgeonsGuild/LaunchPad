using Grpc.Core;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Rocket.Surgery.LaunchPad.Primitives;

namespace Rocket.Surgery.LaunchPad.Grpc;

internal class NotAuthorizedInterceptor(IOptions<JsonOptions> options) : ProblemDetailsInterceptor<NotAuthorizedException>(StatusCode.PermissionDenied, options.Value.SerializerOptions);
