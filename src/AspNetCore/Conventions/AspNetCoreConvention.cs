using System.Reflection;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.DependencyInjection.Compiled;
using Rocket.Surgery.LaunchPad.AspNetCore.Filters;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Conventions;

/// <summary>
///     Class MvcConvention.
/// </summary>
/// <seealso cref="IServiceConvention" />
/// <remarks>
///     Builds the aspnet core convention
/// </remarks>
/// <param name="options"></param>
[PublicAPI]
[ExportConvention]
[ConventionCategory(ConventionCategory.Application)]
[RequiresUnreferencedCode("Registering the MVC services requires access to the application's compiled assemblies.")]
public class AspNetCoreConvention(AspNetCoreOptions? options = null) : IServiceConvention
{
    /// <summary>
    ///     Registers the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="configuration"></param>
    /// <param name="services"></param>
    /// TODO Edit XML Comment Template for Register
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(context);

        services
           .AddEndpointsApiExplorer()
           .AddMvcCore()
           .AddApiExplorer();
        PopulateDefaultParts(
            // ReSharper disable once NullableWarningSuppressionIsUsed
            GetServiceFromCollection<ApplicationPartManager>(services)!,
            context
               .Assembly.GetCompiledTypeProvider()
               .GetAssemblies(s => s.FromAssemblyDependenciesOf<AspNetCoreConvention>())
               .Where(_options.AssemblyPartFilter)
               .SelectMany(GetApplicationPartAssemblies)
        );

        services.Configure<MvcOptions>(
            options =>
            {
                options.Filters.Add<NotFoundExceptionFilter>();
                options.Filters.Add<NotAuthorizedExceptionFilter>();
                options.Filters.Add<RequestFailedExceptionFilter>();
                options.Filters.Add<SerilogLoggingActionFilter>(0);
                options.Filters.Add<SerilogLoggingPageFilter>(0);
            }
        );
    }

    internal static void PopulateDefaultParts(
        ApplicationPartManager manager,
        IEnumerable<Assembly> assemblies
    )
    {
        var seenAssemblies = new HashSet<Assembly>();

        foreach (var assembly in assemblies)
        {
            if (!seenAssemblies.Add(assembly))
                // "assemblies" may contain duplicate values, but we want unique ApplicationPart instances.
                // Note that we prefer using a HashSet over Distinct since the latter isn't
                // guaranteed to preserve the original ordering.
                continue;

            var partFactory = ApplicationPartFactory.GetApplicationPartFactory(assembly);
            foreach (var applicationPart in partFactory.GetApplicationParts(assembly))
            {
                manager.ApplicationParts.Add(applicationPart);
            }
        }
    }

    private static T? GetServiceFromCollection<T>(IServiceCollection services) => (T?)services
                                                                                     .LastOrDefault(d => d.ServiceType == typeof(T))
                                                                                    ?.ImplementationInstance;

    private static IEnumerable<Assembly> GetApplicationPartAssemblies(Assembly assembly)
    {
        // Use ApplicationPartAttribute to get the closure of direct or transitive dependencies
        // that reference MVC.
        var assembliesFromAttributes = assembly
                                      .GetCustomAttributes<ApplicationPartAttribute>()
                                      .Select(name => Assembly.Load(name.AssemblyName))
                                      .OrderBy(a => a.FullName, StringComparer.Ordinal)
                                      .SelectMany(GetAssemblyClosure);

        // The SDK will not include the entry assembly as an application part. We'll explicitly list it
        // and have it appear before all other assemblies \ ApplicationParts.
        return GetAssemblyClosure(assembly)
           .Concat(assembliesFromAttributes);
    }

    private static IEnumerable<Assembly> GetAssemblyClosure(Assembly assembly)
    {
        yield return assembly;

        var relatedAssemblies = RelatedAssemblyAttribute
                               .GetRelatedAssemblies(assembly, false)
                               .OrderBy(a => a.FullName, StringComparer.Ordinal);

        foreach (var relatedAssembly in relatedAssemblies)
        {
            yield return relatedAssembly;
        }
    }

    private readonly AspNetCoreOptions _options = options ?? new AspNetCoreOptions();
}
