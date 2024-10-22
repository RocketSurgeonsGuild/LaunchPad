using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Setup;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Conventions;

/// <summary>
///     ProblemDetailsConvention.
///     Implements the <see cref="IServiceConvention" />
/// </summary>
/// <seealso cref="IServiceConvention" />
/// <seealso cref="IServiceConvention" />
[PublicAPI]
[ExportConvention]
[BeforeConvention(typeof(Foundation.Conventions.FluentValidationConvention))]
[ConventionCategory(ConventionCategory.Application)]
public class AspNetCoreValidationBehaviorConvention : ISetupConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context)
    {
        context.Set("RegisterValidationOptionsAsHealthChecks", true);
    }
}
