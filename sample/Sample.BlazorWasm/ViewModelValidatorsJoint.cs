using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;
using Microsoft.Extensions.DependencyInjection.Extensions;

using ZeroAlloc.Validation;

namespace Sample.BlazorWasm;

/// <summary>
///     Registers the validator of the view model the form binds to. ZeroAllocValidator requires a registered validator for the form's model,
///     and ZeroAlloc.Validation registers only those of the <c>[Validate]</c> types; the view model's <c>Validator</c>, which <c>[Mutable]</c>
///     writes, validates the request it makes.
/// </summary>
[DovetailExport]
internal sealed class ViewModelValidatorsJoint : IServiceJoint
{
    public void Register(IDovetailContext context, IServiceCollection services) =>
        services.TryAddSingleton<ValidatorFor<TestHandler.RequestViewModel>, TestHandler.RequestViewModel.Validator>();
}
