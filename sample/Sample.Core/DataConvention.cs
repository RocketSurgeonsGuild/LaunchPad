using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Sample.Core;
using Sample.Core.Domain;
using IConventionContext = Rocket.Surgery.Conventions.IConventionContext;

[assembly: Convention(typeof(DataConvention))]
namespace Sample.Core
{
    class DataConvention : IServiceConvention
    {
        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            services.AddPooledDbContextFactory<RocketDbContext>(x => x
               .EnableDetailedErrors()
               .EnableSensitiveDataLogging()
               .EnableServiceProviderCaching()
               .UseSqlite(connection)
            );
        }
    }
}