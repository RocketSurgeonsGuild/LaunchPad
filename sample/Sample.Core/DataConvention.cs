using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Sample.Core.Domain;
#if NET
using Microsoft.Extensions.DependencyInjection.Extensions;
#endif

namespace Sample.Core;

[LiveConvention]
[ExportConvention]
internal class DataConvention : IServiceConvention
{
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
#pragma warning disable CA2000
        var connection = new SqliteConnection("DataSource=:memory:");
#pragma warning restore CA2000
        connection.Open();
        services
           .AddDbContextPool<RocketDbContext>(
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
        services.TryAddScoped(provider => provider.GetRequiredService<IDbContextFactory<RocketDbContext>>().CreateDbContext());
#endif
    }
}
