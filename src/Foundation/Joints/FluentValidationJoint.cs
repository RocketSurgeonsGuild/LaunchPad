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
///         The application seeds it with <c>builder.Set&lt;ISpecularProvider&gt;(SpecularProvider.Instance)</c> until Dovetail does.
///     </para>
///     <para>
///         The <see cref="FoundationOptions" /> are read from the joint context when the joint registers, and default when none were set.
///         This stays out of the constructor until Dovetail resolves constructor parameters of exported joints from the context.
///     </para>
/// </remarks>
/// <seealso cref="IServiceJoint" />
/// <seealso cref="MediatRJoint" />
[PublicAPI]
[DovetailExport]
[AfterJoint<MediatRJoint>]
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

        var options = context.Get<FoundationOptions>() ?? new FoundationOptions();

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
        services.TryAddEnumerable(ServiceDescriptor.Describe(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>), options.MediatorLifetime));
        services.TryAddEnumerable(ServiceDescriptor.Describe(typeof(IStreamPipelineBehavior<,>), typeof(ValidationStreamPipelineBehavior<,>), options.MediatorLifetime));
    }
}
