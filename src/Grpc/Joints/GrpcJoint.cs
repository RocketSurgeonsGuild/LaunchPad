using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.Grpc.Validation;

namespace Rocket.Surgery.LaunchPad.Grpc.Joints;

/// <summary>
///     Registers gRPC with the LaunchPad exception interceptors and message validation.
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
           .AddGrpcValidation()
           .AddGrpc(
                options =>
                {
                    options.EnableMessageValidation();
                    options.Interceptors.Add<NotAuthorizedInterceptor>();
                    options.Interceptors.Add<NotFoundInterceptor>();
                    options.Interceptors.Add<RequestFailedInterceptor>();
                }
            )
            ;
    }
}
