using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Rocket.Surgery.LaunchPad.HotChocolate.FairyBread;

public static class IRequestExecutorBuilderExtensions
{
    public static IRequestExecutorBuilder AddFairyBread(
        this IRequestExecutorBuilder requestExecutorBuilder,
        Action<IFairyBreadOptions>? configureOptions = null)
    {
        // Services
        var services = requestExecutorBuilder.Services;

        var options = new DefaultFairyBreadOptions();
        configureOptions?.Invoke(options);

        services.TryAddSingleton<IFairyBreadOptions>(options);
        services.TryAddSingleton<IValidatorRegistry, DefaultValidatorRegistry>();
        services.TryAddSingleton<IValidatorProvider, DefaultValidatorProvider>();
        services.TryAddSingleton<IValidationErrorsHandler, DefaultValidationErrorsHandler>();

        // Executor builder
        requestExecutorBuilder.TryAddTypeInterceptor<ValidationMiddlewareInjector>();

        return requestExecutorBuilder;
    }
}
