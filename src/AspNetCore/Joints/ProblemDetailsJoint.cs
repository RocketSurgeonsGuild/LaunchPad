using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

/// <summary>
///     Registers problem details. Rocket.Surgery.LaunchPad.AspNetCore.Mvc also fills in their title and type from MVC's client error mapping.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Application)]
public class ProblemDetailsJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services) => services.AddProblemDetails();
}
