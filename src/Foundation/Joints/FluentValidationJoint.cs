using FluentValidation;

using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.Foundation.Validation;

namespace Rocket.Surgery.LaunchPad.Foundation.Joints;

/// <summary>
///     A joint that registers every FluentValidation validator found in the assemblies that depend on FluentValidation,
///     the validators for options, and the MediatR validation pipeline behaviors.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <remarks>
///     <para>
///         Runs after the <see cref="MediatRJoint" />, so the validation behaviors are added to the pipeline MediatR registers.
///     </para>
///     <para>
///         The validators are found with the <see cref="ISpecularProvider" /> in the joint context.
///         The generated <c>Imports</c> seed the application's Specular provider into the context.
///     </para>
///     <para>
///         Dovetail resolves the <see cref="FoundationOptions" /> from the joint context when it creates the joint, and they default when none
///         were set.
///     </para>
/// </remarks>
/// <param name="options">The options, or <see langword="null" /> for the defaults.</param>
/// <seealso cref="IServiceJoint" />
/// <seealso cref="MediatRJoint" />
[PublicAPI]
[DovetailExport]
[AfterJoint<MediatRJoint>]
[DovetailCategory(DovetailCategory.Core)]
public class FluentValidationJoint(FoundationOptions? options = null) : IServiceJoint
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
        services.TryAddEnumerable(ServiceDescriptor.Describe(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>), _options.MediatorLifetime));
        services.TryAddEnumerable(ServiceDescriptor.Describe(typeof(IStreamPipelineBehavior<,>), typeof(ValidationStreamPipelineBehavior<,>), _options.MediatorLifetime));
    }

    private readonly FoundationOptions _options = options ?? new FoundationOptions();
}
