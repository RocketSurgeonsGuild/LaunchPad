using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.AspNetCore;

/// <summary>
///     Helper methods for getting an mvc builder if you're not sure that mvc has been added to the application yet.
/// </summary>
public static class RocketSurgeryMvcCoreExtensions
{
    /// <summary>
    ///     Gets an <see cref="IMvcBuilder" /> with no side-effects
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IMvcBuilder WithMvc(this IServiceCollection services)
    {
        return new ImmutableMvcBuilder(services, false);
    }

    /// <summary>
    ///     Gets an <see cref="IMvcCoreBuilder" /> with no side-effects
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IMvcCoreBuilder WithMvcCore(this IServiceCollection services)
    {
        return new ImmutableMvcBuilder(services, true);
    }

    /// <summary>
    ///     AddMvc or AddMvcCore can cause breaking changes if called multiple times over the application lifespan.
    ///     This allows us to ensure that we don't upset that state, but get back the expected mvc builder.
    /// </summary>
    private class ImmutableMvcBuilder : IMvcBuilder, IMvcCoreBuilder
    {
        private static ApplicationPartManager GetApplicationPartManager(IServiceCollection services, bool core)
        {
            return GetServiceFromCollection<ApplicationPartManager>(services) ?? ( core ? services.AddMvcCore().PartManager : services.AddMvc().PartManager );
        }

        private static T? GetServiceFromCollection<T>(IServiceCollection services)
            where T : class
        {
            return (T?)services.LastOrDefault(d => d.ServiceType == typeof(T))?.ImplementationInstance;
        }

        private readonly Lazy<ApplicationPartManager> _partsManager;

        public ImmutableMvcBuilder(IServiceCollection services, bool core)
        {
            Services = services;
            _partsManager = new Lazy<ApplicationPartManager>(() => GetApplicationPartManager(services, core));
        }

        public ApplicationPartManager PartManager => _partsManager.Value;
        public IServiceCollection Services { get; }
    }
}
