using System.Reflection;

using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.AspNetCore.Filters;

using Specular;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

/// <summary>
///     Registers MVC, the API explorer and the application parts for every assembly that depends on this one.
///     The other ASP.NET Core joints run after this one.
/// </summary>
/// <seealso cref="IServiceJoint" />
/// <remarks>
///     The <see cref="AspNetCoreOptions" /> are read from the Joint context when this joint registers; without them the
///     defaults apply.
/// </remarks>
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Application)]
[RequiresUnreferencedCode("Registering the MVC services requires access to the application's compiled assemblies.")]
public class AspNetCoreJoint : IServiceJoint
{
    /// <summary>
    ///     Registers the MVC services, and adds the application parts found through the Specular provider in the context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="services">The service collection.</param>
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(context);

        var aspNetCoreOptions = context.Get<AspNetCoreOptions>() ?? new AspNetCoreOptions();

        services
           .AddEndpointsApiExplorer()
           .AddMvcCore()
           .AddApiExplorer();
        PopulateDefaultParts(
            // ReSharper disable once NullableWarningSuppressionIsUsed
            GetServiceFromCollection<ApplicationPartManager>(services)!,
            context
               .Require<ISpecularProvider>()
               .GetAssemblies(s => s.DependenciesFromAssemblyOf<AspNetCoreJoint>())
               .Where(aspNetCoreOptions.AssemblyPartFilter)
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
}
