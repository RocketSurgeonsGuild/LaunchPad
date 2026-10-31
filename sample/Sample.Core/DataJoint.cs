using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Sample.Core.Domain;

namespace Sample.Core;

/// <summary>
///     Registers the in memory database the samples run against. Unit tests bring their own.
/// </summary>
[LiveJoint]
[DovetailExport]
internal class DataJoint : IServiceJoint
{
    public void Register(IDovetailContext context, IServiceCollection services)
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

        // temp?
        services.TryAddScoped(provider => provider.GetRequiredService<IDbContextFactory<RocketDbContext>>().CreateDbContext());
    }
}
