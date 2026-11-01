using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.Grpc.Joints;

/// <summary>
///     Registers gRPC with the LaunchPad exception interceptors.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Application)]
public class GrpcJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        services
           .AddGrpc(
                options =>
                {
                    options.Interceptors.Add<NotAuthorizedInterceptor>();
                    options.Interceptors.Add<NotFoundInterceptor>();
                    options.Interceptors.Add<RequestFailedInterceptor>();
                }
            )
            ;
    }
}
