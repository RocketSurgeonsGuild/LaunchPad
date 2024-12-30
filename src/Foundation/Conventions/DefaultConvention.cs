using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Setup;
using Rocket.Surgery.DependencyInjection.Compiled;

namespace Rocket.Surgery.LaunchPad.Foundation.Conventions;

/// <summary>
///     MediatRConvention.
///     Implements the <see cref="IServiceConvention" />
/// </summary>
/// <seealso cref="IServiceConvention" />
[PublicAPI]
[ExportConvention]
[ConventionCategory(ConventionCategory.Core)]

public class DefaultConvention : IServiceConvention, ISetupConvention
{

    /// <summary>
    ///     Registers the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="configuration"></param>
    /// <param name="services"></param>
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        services
           .AddOptions()
           .AddLogging()
           .AddExecuteScopedServices();

        services.AddCompiledServiceRegistrations(context.Assembly.GetCompiledTypeProvider());
    }

    void ISetupConvention.Register(IConventionContext context) => context.AddIfMissing("ExecutingAssembly", context.Require<LoadConventions>().Method.Module.Assembly);
}
