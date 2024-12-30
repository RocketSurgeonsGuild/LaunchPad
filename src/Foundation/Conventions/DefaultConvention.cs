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
[System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
public class DefaultConvention : IServiceConvention, ISetupConvention
{
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private string DebuggerDisplay
    {
        get
        {
            return ToString();
        }
    }

    /// <summary>
    ///     Registers the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="configuration"></param>
    /// <param name="services"></param>
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        _ = services
           .AddOptions()
           .AddLogging()
           .AddExecuteScopedServices();

        _ = services.AddCompiledServiceRegistrations(context.Assembly.GetCompiledTypeProvider());
    }

    void ISetupConvention.Register(IConventionContext context) => context.AddIfMissing("ExecutingAssembly", context.Require<LoadConventions>().Method.Module.Assembly);
}
