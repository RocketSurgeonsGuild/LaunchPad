using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.HotChocolate.Configuration;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Joints;

/// <summary>
///     Hot Chocolate Joint
/// </summary>
/// <remarks>
///     The NodaTime types come from the <c>NodaTimeHotChocolateJoint</c> in Rocket.Surgery.LaunchPad.NodaTime.HotChocolate.
/// </remarks>
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Application)]
public class HotChocolateJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        services
           .ConfigureOptions<HotChocolateContextDataConfigureOptions>();
        context
           .GetOrAdd(() => services.AddGraphQL())
           .AddInstrumentation();
    }
}
