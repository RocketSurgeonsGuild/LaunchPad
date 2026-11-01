using Grpc.AspNetCore.Server;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.Grpc.Validation;

namespace Rocket.Surgery.LaunchPad.Grpc.Joints;

/// <summary>
///     Validates gRPC request messages with FluentValidation.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <remarks>
///     Runs before the <see cref="GrpcJoint" />, so the validation interceptor comes before its exception interceptors.
/// </remarks>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[BeforeJoint<GrpcJoint>]
[DovetailCategory(DovetailCategory.Application)]
public class FluentValidationGrpcJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        services
           .AddGrpcValidation()
           .Configure<GrpcServiceOptions>(options => options.EnableMessageValidation());
    }
}
