using System.Reflection;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using NodaTime;
using NodaTime.Extensions;

namespace Rocket.Surgery.LaunchPad.HotChocolate;

/// <summary>
///     The assembly info object
/// </summary>
[PublicAPI]
public class AssemblyInfo
{
    private readonly Assembly _rootAssembly;

    /// <summary>
    ///     The assembly info for the given assembly
    /// </summary>
    /// <param name="rootAssembly"></param>
    public AssemblyInfo(Assembly rootAssembly)
    {
        _rootAssembly = rootAssembly;
    }

    /// <summary>
    ///     The assembly version
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<string?> Version(IResolverContext context, CancellationToken cancellationToken)
    {
        return context.CacheDataLoader<string, string?>(
            (key, ct) =>
                Task.FromResult<string?>(
                    _rootAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                 ?? _rootAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version
                 ?? _rootAssembly.GetCustomAttribute<AssemblyVersionAttribute>()?.Version
                 ?? "0.0.0"
                ),
            nameof(Version)
        ).LoadAsync(nameof(Version), cancellationToken);
    }

    /// <summary>
    ///     The assembly created date
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Instant?> Created(IResolverContext context, CancellationToken cancellationToken)
    {
        return context.CacheDataLoader<string, Instant?>(
            (key, ct) =>
            {
                var location = _rootAssembly.Location;
                return Task.FromResult<Instant?>(!string.IsNullOrWhiteSpace(location) ? File.GetCreationTimeUtc(location).ToInstant() : null);
            },
            nameof(Created)
        ).LoadAsync(nameof(Created), cancellationToken);
    }

    /// <summary>
    ///     The assembly updated date
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Instant?> Updated(IResolverContext context, CancellationToken cancellationToken)
    {
        return context.CacheDataLoader<string, Instant?>(
            (key, ct) =>
            {
                var location = _rootAssembly.Location;
                return Task.FromResult<Instant?>(!string.IsNullOrWhiteSpace(location) ? File.GetLastWriteTimeUtc(location).ToInstant() : null);
            },
            nameof(Updated)
        ).LoadAsync(nameof(Updated), cancellationToken);
    }


    /// <summary>
    ///     The assembly company
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<string?> Company(IResolverContext context, CancellationToken cancellationToken)
    {
        return context.CacheDataLoader<string, string?>(
            (key, ct) =>
                Task.FromResult(
                    _rootAssembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company
                ),
            nameof(Company)
        ).LoadAsync(nameof(Company), cancellationToken);
    }

    /// <summary>
    ///     The configuration the assembly was built with
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<string?> Configuration(IResolverContext context, CancellationToken cancellationToken)
    {
        return context.CacheDataLoader<string, string?>(
            (key, ct) =>
                Task.FromResult(
                    _rootAssembly.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration
                ),
            nameof(Configuration)
        ).LoadAsync(nameof(Configuration), cancellationToken);
    }

    /// <summary>
    ///     The assembly copyright
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<string?> Copyright(IResolverContext context, CancellationToken cancellationToken)
    {
        return context.CacheDataLoader<string, string?>(
            (key, ct) =>
                Task.FromResult(
                    _rootAssembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright
                ),
            nameof(Copyright)
        ).LoadAsync(nameof(Copyright), cancellationToken);
    }

    /// <summary>
    ///     The assembly description
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<string?> Description(IResolverContext context, CancellationToken cancellationToken)
    {
        return context.CacheDataLoader<string, string?>(
            (key, ct) =>
                Task.FromResult(
                    _rootAssembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description
                ),
            nameof(Description)
        ).LoadAsync(nameof(Description), cancellationToken);
    }

    /// <summary>
    ///     The assembly product
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<string?> Product(IResolverContext context, CancellationToken cancellationToken)
    {
        return context.CacheDataLoader<string, string?>(
            (key, ct) =>
                Task.FromResult(
                    _rootAssembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product
                ),
            nameof(Product)
        ).LoadAsync(nameof(Product), cancellationToken);
    }

    /// <summary>
    ///     The assembly title
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<string?> Title(IResolverContext context, CancellationToken cancellationToken)
    {
        return context.CacheDataLoader<string, string?>(
            (key, ct) =>
                Task.FromResult(
                    _rootAssembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title
                ),
            nameof(Title)
        ).LoadAsync(nameof(Title), cancellationToken);
    }

    /// <summary>
    ///     The assembly trademark
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<string?> Trademark(IResolverContext context, CancellationToken cancellationToken)
    {
        return context.CacheDataLoader<string, string?>(
            (key, ct) =>
                Task.FromResult(
                    _rootAssembly.GetCustomAttribute<AssemblyTrademarkAttribute>()?.Trademark
                ),
            nameof(Trademark)
        ).LoadAsync(nameof(Trademark), cancellationToken);
    }

    /// <summary>
    ///     The assembly metadata
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<IDictionary<string, string?>> Metadata(IResolverContext context, CancellationToken cancellationToken)
    {
        return context
              .CacheDataLoader<string, IDictionary<string, string?>>(
                   (key, ct) =>
                       Task.FromResult<IDictionary<string, string?>>(
                           _rootAssembly.GetCustomAttributes<AssemblyMetadataAttribute>().ToDictionary(z => z.Key, z => z.Value)
                       ),
                   nameof(Metadata)
               ).LoadAsync(nameof(Metadata), cancellationToken);
    }
}
