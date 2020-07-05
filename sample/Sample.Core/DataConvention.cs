using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions.DependencyInjection;
using Sample.Core.Domain;

namespace Sample.Core
{
    class DataConvention : IServiceConvention
    {
        public void Register(IServiceConventionContext context)
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            context.Services
               .AddDbContext<RocketDbContext>(x => SqliteDbContextOptionsBuilderExtensions.UseSqlite(
                        x
                           .EnableDetailedErrors()
                           .EnableSensitiveDataLogging()
                           .EnableServiceProviderCaching(), connection)
                );
        }
    }
}