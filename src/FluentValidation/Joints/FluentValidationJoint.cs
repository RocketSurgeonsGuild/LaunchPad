using FluentValidation;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.Foundation.Validation;

namespace Rocket.Surgery.LaunchPad.Foundation.Joints;

/// <summary>
///     A joint that registers every FluentValidation validator found in the assemblies that depend on FluentValidation and
///     the validators for options.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <remarks>
///     <para>
///         The validators are found with the <see cref="ISpecularProvider" /> in the joint context.
///         The generated <c>Imports</c> seed the application's Specular provider into the context.
///     </para>
/// </remarks>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Core)]
public class FluentValidationJoint : IServiceJoint
{
    /// <summary>
    ///     Registers the specified context.
    /// </summary>
    /// <param name="context">The joint context.</param>
    /// <param name="services">The service collection.</param>
    /// <exception cref="KeyNotFoundException">No <see cref="ISpecularProvider" /> was set in <paramref name="context" />.</exception>
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(services);

        context
           .Require<ISpecularProvider>()
           .Scan(
                services,
                z => z
                    .DependenciesFromAssemblyOf<IValidator>()
                    .AddClasses(t => t.AssignableTo<IValidator>().NotAssignableTo(typeof(CompositeValidator<>)))
                    .AsSelf()
                    .AsImplementedInterfaces(a => a.AssignableTo<IValidator>())
                    .WithTransientLifetime()
            );

        services.AddSingleton(typeof(IValidateOptions<>), typeof(FluentValidationOptions<>));
    }
}
