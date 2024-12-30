using System.Globalization;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.DependencyInjection.Compiled;
using Rocket.Surgery.LaunchPad.Foundation.Validation;

namespace Rocket.Surgery.LaunchPad.Foundation.Conventions;

/// <summary>
///     ValidationConvention.
///     Implements the <see cref="IServiceConvention" />
/// </summary>
/// <seealso cref="IServiceConvention" />
/// <seealso cref="IServiceConvention" />
/// <remarks>
///     Create the fluent validation convention
/// </remarks>
/// <param name="options"></param>
[PublicAPI]
[ExportConvention]
[AfterConvention(typeof(MediatRConvention))]
[AfterConvention(typeof(HealthChecksConvention))]
[ConventionCategory(ConventionCategory.Core)]
[System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
public class FluentValidationConvention(FoundationOptions? options = null) : IServiceConvention
{
    private readonly FoundationOptions _options = options ?? new FoundationOptions();

    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString();

    /// <summary>
    ///     Registers the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="configuration"></param>
    /// <param name="services"></param>
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(context);

        _ = context.Assembly.GetCompiledTypeProvider()
               .Scan(
                    services,
                    z => z
                        .FromAssemblyDependenciesOf<IValidator>()
                        .AddClasses(t => t.AssignableTo<IValidator>().NotAssignableTo(typeof(CompositeValidator<>)))
                        .AsSelf()
                        .AsImplementedInterfaces(a => a.AssignableTo<IValidator>())
                        .WithTransientLifetime()
                );

        if (_options.RegisterValidationOptionsAsHealthChecks == true
         || ( !_options.RegisterValidationOptionsAsHealthChecks.HasValue
             && Convert.ToBoolean(
                    context.Properties["RegisterValidationOptionsAsHealthChecks"],
                    CultureInfo.InvariantCulture
                ) )
         || Environment.CommandLine.Contains(
                "microsoft.extensions.apidescription.server",
                StringComparison.OrdinalIgnoreCase
            ))
        {
            // need to do validations using ValidateOnStart
            _ = services.Decorate<HealthCheckService, CustomHealthCheckService>();
            _ = services.AddSingleton<ValidationHealthCheckResults>();
            _ = services.AddSingleton(typeof(IValidateOptions<>), typeof(HealthCheckFluentValidationOptions<>));
        }
        else
        {
            _ = services.AddSingleton(typeof(IValidateOptions<>), typeof(FluentValidationOptions<>));
        }

        services.TryAddEnumerable(ServiceDescriptor.Describe(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>), _options.MediatorLifetime));
        services.TryAddEnumerable(
            ServiceDescriptor.Describe(typeof(IStreamPipelineBehavior<,>), typeof(ValidationStreamPipelineBehavior<,>), _options.MediatorLifetime)
        );
    }
}
