using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Sample.Core;
using Sample.Core.Domain;
#if NET
using Microsoft.Extensions.DependencyInjection.Extensions;
#endif

[assembly: Convention(typeof(DataConvention))]

namespace Sample.Core;

internal class DataConvention : IServiceConvention
{
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
#pragma warning disable CA2000
        var connection = new SqliteConnection("DataSource=:memory:");
#pragma warning restore CA2000
        connection.Open();
        services
#if NETSTANDARD
           .AddDbContextPool<RocketDbContext>(
#else
           .AddPooledDbContextFactory<RocketDbContext>(
#endif
                x => x
                    .ReplaceService<IValueConverterSelector, StronglyTypedIdValueConverterSelector>()
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging()
                    .EnableServiceProviderCaching().UseSqlite(
                         connection
                     )
            );
#if NET
        // temp?
        services.TryAddScoped(_ => _.GetRequiredService<IDbContextFactory<RocketDbContext>>().CreateDbContext());
#endif
    }
}
