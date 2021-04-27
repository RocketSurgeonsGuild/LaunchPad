using HotChocolate.Resolvers;
using HotChocolate.Types;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Extensions;
using Rocket.Surgery.LaunchPad.Foundation;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Rocket.Surgery.LaunchPad.HotChocolate
{
    public class RocketChocolateOptions
    {
        public bool IncludeAssemblyInfoQuery { get; set; }
    }


    [ExtendObjectType(OperationTypeNames.Query)]
    [PublicAPI]
    public class AssemblyInfoQuery
    {
        public AssemblyInfo Version(IResolverContext context, CancellationToken cancellationToken) => new(
            context.Services.GetService<FoundationOptions>()?.EntryAssembly ?? Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()
        );
    }

    [PublicAPI]
    public class AssemblyInfo
    {
        private readonly Assembly _rootAssembly;

        public AssemblyInfo(Assembly rootAssembly) => _rootAssembly = rootAssembly;

        public Task<string?> Version(IResolverContext context, CancellationToken cancellationToken) => context.CacheDataLoader<string, string?>(
            (key, ct) =>
                Task.FromResult<string?>(
                    _rootAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                 ?? _rootAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version
                 ?? _rootAssembly.GetCustomAttribute<AssemblyVersionAttribute>()?.Version
                 ?? "0.0.0"
                ),
            nameof(Version)
        ).LoadAsync(nameof(Version), cancellationToken);

        public Task<Instant?> Created(IResolverContext context, CancellationToken cancellationToken) => context.CacheDataLoader<string, Instant?>(
            (key, ct) =>
            {
                var location = _rootAssembly.Location;
                return Task.FromResult<Instant?>(!string.IsNullOrWhiteSpace(location) ? File.GetCreationTimeUtc(location).ToInstant() : null);
            },
            nameof(Created)
        ).LoadAsync(nameof(Created), cancellationToken);

        public Task<Instant?> Updated(IResolverContext context, CancellationToken cancellationToken) => context.CacheDataLoader<string, Instant?>(
            (key, ct) =>
            {
                var location = _rootAssembly.Location;
                return Task.FromResult<Instant?>(!string.IsNullOrWhiteSpace(location) ? File.GetLastWriteTimeUtc(location).ToInstant() : null);
            },
            nameof(Updated)
        ).LoadAsync(nameof(Updated), cancellationToken);


        public Task<string?> Company(IResolverContext context, CancellationToken cancellationToken) => context.CacheDataLoader<string, string?>(
            (key, ct) =>
                Task.FromResult(
                    _rootAssembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company
                ),
            nameof(Company)
        ).LoadAsync(nameof(Company), cancellationToken);

        public Task<string?> Configuration(IResolverContext context, CancellationToken cancellationToken) => context.CacheDataLoader<string, string?>(
            (key, ct) =>
                Task.FromResult(
                    _rootAssembly.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration
                ),
            nameof(Configuration)
        ).LoadAsync(nameof(Configuration), cancellationToken);

        public Task<string?> Copyright(IResolverContext context, CancellationToken cancellationToken) => context.CacheDataLoader<string, string?>(
            (key, ct) =>
                Task.FromResult(
                    _rootAssembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright
                ),
            nameof(Copyright)
        ).LoadAsync(nameof(Copyright), cancellationToken);

        public Task<string?> Description(IResolverContext context, CancellationToken cancellationToken) => context.CacheDataLoader<string, string?>(
            (key, ct) =>
                Task.FromResult(
                    _rootAssembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description
                ),
            nameof(Description)
        ).LoadAsync(nameof(Description), cancellationToken);

        public Task<string?> Product(IResolverContext context, CancellationToken cancellationToken) => context.CacheDataLoader<string, string?>(
            (key, ct) =>
                Task.FromResult<string?>(
                    _rootAssembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product
                ),
            nameof(Product)
        ).LoadAsync(nameof(Product), cancellationToken);

        public Task<string?> Title(IResolverContext context, CancellationToken cancellationToken) => context.CacheDataLoader<string, string?>(
            (key, ct) =>
                Task.FromResult<string?>(
                    _rootAssembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title
                ),
            nameof(Title)
        ).LoadAsync(nameof(Title), cancellationToken);

        public Task<string?> Trademark(IResolverContext context, CancellationToken cancellationToken) => context.CacheDataLoader<string, string?>(
            (key, ct) =>
                Task.FromResult<string?>(
                    _rootAssembly.GetCustomAttribute<AssemblyTrademarkAttribute>()?.Trademark
                ),
            nameof(Trademark)
        ).LoadAsync(nameof(Trademark), cancellationToken);

        public Task<IDictionary<string, string>> Metadata(IResolverContext context, CancellationToken cancellationToken) => context
           .CacheDataLoader<string, IDictionary<string, string>>(
                (key, ct) =>
                    Task.FromResult<IDictionary<string, string>>(
                        _rootAssembly.GetCustomAttributes<AssemblyMetadataAttribute>().ToDictionary(z => z.Key, z => z.Value)
                    ),
                nameof(Metadata)
            ).LoadAsync(nameof(Metadata), cancellationToken);
    }
}