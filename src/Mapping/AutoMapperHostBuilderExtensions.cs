using Rocket.Surgery.LaunchPad.Mapping;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions;

/// <summary>
///     AutoMapperHostBuilderExtensions.
/// </summary>
[PublicAPI]
public static class AutoMapperHostBuilderExtensions
{
    /// <summary>
    ///     Uses AutoMapper.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="options">The options object</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder UseAutoMapper(this ConventionContextBuilder container, AutoMapperOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(container);

        container.PrependConvention<AutoMapperConvention>();
        return container;
    }
}
