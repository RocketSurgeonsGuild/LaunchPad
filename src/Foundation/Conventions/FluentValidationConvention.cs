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
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.LaunchPad.Foundation.Validation;

namespace Rocket.Surgery.LaunchPad.Foundation.Conventions;

/// <summary>
///     ValidationConvention.
///     Implements the <see cref="IServiceConvention" />
/// </summary>
/// <seealso cref="IServiceConvention" />
/// <seealso cref="IServiceConvention" />
[PublicAPI]
[ExportConvention]
[AfterConvention(typeof(MediatRConvention))]
[AfterConvention(typeof(HealthChecksConvention))]
public class FluentValidationConvention : IServiceConvention
{
    private readonly FoundationOptions _options;

    /// <summary>
    ///     Create the fluent validation convention
    /// </summary>
    /// <param name="options"></param>
    public FluentValidationConvention(FoundationOptions? options = null)
    {
        _options = options ?? new FoundationOptions();
    }

    /// <summary>
    ///     Registers the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="configuration"></param>
    /// <param name="services"></param>
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var types = context
                   .AssemblyProvider.GetTypes(
                        z => z
                            .FromAssemblyDependenciesOf<IValidator>()
                            .GetTypes(
                                 f => f
                                     .AssignableTo(typeof(AbstractValidator<>))
                                     .NotInfoOf(TypeInfoFilter.GenericType, TypeInfoFilter.Abstract)
                             )
                    );
        foreach (var validator in types)
        {
            if (validator is not { BaseType: { IsGenericType: true, GenericTypeArguments: [var innerType,], }, }) continue;
            var interfaceType = typeof(IValidator<>).MakeGenericType(innerType);
            services.Add(new(interfaceType, validator, _options.ValidatorLifetime));
            services.Add(new(validator, validator, _options.ValidatorLifetime));
        }

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
            services.Decorate<HealthCheckService, CustomHealthCheckService>();
            services.AddSingleton<ValidationHealthCheckResults>();
            services.AddSingleton(typeof(IValidateOptions<>), typeof(HealthCheckFluentValidationOptions<>));
        }
        else
        {
            services.AddSingleton(typeof(IValidateOptions<>), typeof(FluentValidationOptions<>));
        }

        services.TryAddEnumerable(ServiceDescriptor.Describe(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>), _options.MediatorLifetime));
        services.TryAddEnumerable(
            ServiceDescriptor.Describe(typeof(IStreamPipelineBehavior<,>), typeof(ValidationStreamPipelineBehavior<,>), _options.MediatorLifetime)
        );
    }
}