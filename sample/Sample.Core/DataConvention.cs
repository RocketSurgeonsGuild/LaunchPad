using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
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
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        services
#if NETSTANDARD
           .AddDbContextPool<RocketDbContext>(
#else
           .AddPooledDbContextFactory<RocketDbContext>(
#endif
                x => SqliteDbContextOptionsBuilderExtensions.UseSqlite(
                    x
                       .EnableDetailedErrors()
                       .EnableSensitiveDataLogging()
                       .EnableServiceProviderCaching(), connection
                )
            );
#if NET
        // temp?
        services.TryAddScoped(_ => _.GetRequiredService<IDbContextFactory<RocketDbContext>>().CreateDbContext());
#endif
    }
}
