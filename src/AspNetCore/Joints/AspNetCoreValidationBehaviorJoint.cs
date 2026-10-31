using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using FoundationFluentValidationJoint = Rocket.Surgery.LaunchPad.Foundation.Joints.FluentValidationJoint;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

/// <summary>
///     Asks for the validation options to be registered as health checks.
///     Implements the <see cref="ISetupJoint" />
/// </summary>
/// <seealso cref="ISetupJoint" />
[PublicAPI]
[DovetailExport]
[BeforeJoint<FoundationFluentValidationJoint>]
[DovetailCategory(DovetailCategory.Application)]
public class AspNetCoreValidationBehaviorJoint : ISetupJoint
{
    /// <summary>
    ///     The key in the Joint context that carries the request to register the validation options as health checks.
    /// </summary>
    internal const string RegisterValidationOptionsAsHealthChecksKey = "RegisterValidationOptionsAsHealthChecks";

    /// <inheritdoc />
    public void Register(IDovetailContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.Set(RegisterValidationOptionsAsHealthChecksKey, true);
    }
}
