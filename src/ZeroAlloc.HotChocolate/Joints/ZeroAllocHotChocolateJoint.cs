using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.HotChocolate.Validation;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Joints;

/// <summary>
///     Reports each failure of a request the ZeroAlloc mediator rejected as a GraphQL <c>VALIDATION</c> error.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <remarks>
///     Adds the <see cref="ZeroAllocValidationErrorFilter" /> to the GraphQL builder the other GraphQL joints share. Runs after the
///     <see cref="GraphqlJoint" />, so the filter comes after LaunchPad's error filter, which rebuilds every error from its exception and would
///     collapse the one error per failure this filter reports.
/// </remarks>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[AfterJoint<GraphqlJoint>]
[DovetailCategory(DovetailCategory.Application)]
public class ZeroAllocHotChocolateJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services) =>
        context
           .GetOrAdd(() => services.AddGraphQL())
           .AddErrorFilter<ZeroAllocValidationErrorFilter>();
}
