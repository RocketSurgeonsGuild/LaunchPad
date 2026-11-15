using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.Grpc.Joints;

/// <summary>
///     Registers gRPC. An operation reports a failure with a union response, which the generated gRPC servers answer with its status.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Application)]
public class GrpcJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services) => services.AddGrpc();
}
