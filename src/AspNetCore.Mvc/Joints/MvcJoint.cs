using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.AspNetCore.Joints;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Mvc.Joints;

/// <summary>
///     Registers MVC Core, the MVC API explorer and the application parts for every assembly that depends on Rocket.Surgery.LaunchPad.AspNetCore.
///     MVC is not native AOT compatible, which is why this joint lives in Rocket.Surgery.LaunchPad.AspNetCore.Mvc.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <remarks>
///     <para>
///         The application parts are found with the <see cref="ISpecularProvider" /> in the joint context.
///         The generated <c>Imports</c> seed the application's Specular provider into the context.
///     </para>
///     <para>
///         Dovetail resolves the <see cref="AspNetCoreMvcOptions" /> from the joint context when it creates the joint, and they default when none
///         were set.
///     </para>
/// </remarks>
/// <param name="options">The options, or <see langword="null" /> for the defaults.</param>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Application)]
public class MvcJoint(AspNetCoreMvcOptions? options = null) : IServiceJoint
{
    /// <summary>
    ///     Registers the MVC services, and adds the application parts found through the Specular provider in the context.
    /// </summary>
    /// <param name="context">The joint context.</param>
    /// <param name="services">The service collection.</param>
    /// <exception cref="KeyNotFoundException">No <see cref="ISpecularProvider" /> was set in <paramref name="context" />.</exception>
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(services);

        services.AddEndpointsApiExplorer().AddMvcCore().AddApiExplorer();
        PopulateDefaultParts(
            // ReSharper disable once NullableWarningSuppressionIsUsed
            GetServiceFromCollection<ApplicationPartManager>(services)!,
            context
               .Require<ISpecularProvider>()
               .GetAssemblies(s => s.DependenciesFromAssemblyOf<OpenApiJoint>())
               .Where(_options.AssemblyPartFilter)
               .SelectMany(GetApplicationPartAssemblies)
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

    private readonly AspNetCoreMvcOptions _options = options ?? new AspNetCoreMvcOptions();
}
