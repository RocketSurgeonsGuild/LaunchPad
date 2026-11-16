using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Mvc;

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
    /// <remarks>Adds MVC with <c>AddMvc</c> when it hasn't been added, which requires unreferenced code.</remarks>
    [RequiresUnreferencedCode("MVC does not support trimming or native AOT: AddMvc finds controllers and views by reflection.")]
    public static IMvcBuilder WithMvc(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return new ImmutableMvcBuilder(services, () => GetServiceFromCollection<ApplicationPartManager>(services) ?? services.AddMvc().PartManager);
    }

    /// <summary>
    ///     Gets an <see cref="IMvcCoreBuilder" /> with no side-effects
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IMvcCoreBuilder WithMvcCore(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return new ImmutableMvcBuilder(services, () => GetServiceFromCollection<ApplicationPartManager>(services) ?? services.AddMvcCore().PartManager);
    }

    private static T? GetServiceFromCollection<T>(IServiceCollection services)
        where T : class => (T?)services.LastOrDefault(d => d.ServiceType == typeof(T))?.ImplementationInstance;

    /// <summary>
    ///     AddMvc or AddMvcCore can cause breaking changes if called multiple times over the application lifespan.
    ///     This allows us to ensure that we don't upset that state, but get back the expected mvc builder.
    /// </summary>
    private sealed class ImmutableMvcBuilder(IServiceCollection services, Func<ApplicationPartManager> getPartManager) : IMvcBuilder, IMvcCoreBuilder
    {
        private readonly Lazy<ApplicationPartManager> _partsManager = new(getPartManager);

        public ApplicationPartManager PartManager => _partsManager.Value;
        public IServiceCollection Services { get; } = services;
    }
}
