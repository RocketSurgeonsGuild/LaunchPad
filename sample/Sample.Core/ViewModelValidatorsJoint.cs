using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Sample.Core.Operations.Rockets;

using ZeroAlloc.Validation;

namespace Sample.Core;

/// <summary>
///     Registers the validators of the view models the Blazor forms bind to. ZeroAllocValidator requires a registered validator for the
///     form's model, and ZeroAlloc.Validation registers only those of the <c>[Validate]</c> types; each view model's <c>Validator</c>, which
///     <c>[Mutable]</c> writes, validates the request it makes.
/// </summary>
[DovetailExport]
internal sealed class ViewModelValidatorsJoint : IServiceJoint
{
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        services.TryAddSingleton<ValidatorFor<CreateRocket.RequestViewModel>, CreateRocket.RequestViewModel.Validator>();
        services.TryAddSingleton<ValidatorFor<EditRocket.RequestViewModel>, EditRocket.RequestViewModel.Validator>();
    }
}
