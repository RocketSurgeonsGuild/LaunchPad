using Grpc.AspNetCore.Server;

using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.Grpc.Validation;

using ZeroAlloc.Mediator.Validation;

namespace Rocket.Surgery.LaunchPad.Grpc.Joints;

/// <summary>
///     Turns the <see cref="ValidationFailedException" /> the ZeroAlloc mediator throws for an invalid request into
///     an <see cref="global::Grpc.Core.StatusCode.InvalidArgument" /> status.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <remarks>
///     Runs after the <see cref="GrpcJoint" />, so its interceptor is added after, and so runs inside, the exception interceptors that joint adds:
///     the failure is already an <see cref="global::Grpc.Core.RpcException" /> when they see it.
/// </remarks>
/// <seealso cref="IServiceJoint" />
/// <seealso cref="GrpcJoint" />
[PublicAPI]
[DovetailExport]
[AfterJoint<GrpcJoint>]
[DovetailCategory(DovetailCategory.Application)]
public class ZeroAllocGrpcJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services) =>
        services.Configure<GrpcServiceOptions>(options => options.Interceptors.Add<ZeroAllocValidationInterceptor>());
}
