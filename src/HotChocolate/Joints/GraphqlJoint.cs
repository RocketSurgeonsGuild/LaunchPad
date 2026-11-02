using MediatR;

using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.HotChocolate.Types;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Joints;

/// <summary>
///     The graph ql Joint
/// </summary>
/// <remarks>
///     Adds GraphQL with the LaunchPad error filter, and runs before the <see cref="HotChocolateJoint" />.
/// </remarks>
[PublicAPI]
[DovetailExport]
[BeforeJoint<HotChocolateJoint>]
[DovetailCategory(DovetailCategory.Application)]
public class GraphqlJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        context
           .GetOrAdd(() => services.AddGraphQL())
           .AddErrorFilter<GraphqlErrorFilter>()
           .BindRuntimeType<Unit, VoidType>();
    }
}
