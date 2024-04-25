using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Rocket.Surgery.LaunchPad.EntityFramework;

internal static class SqliteDateTimeOffsetModelCreating
{
    public static void OnModelCreating(DbContext context, ModelBuilder modelBuilder)
    {
        if (context.Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            // SQLite does not have proper support for DateTimeOffset via Entity Framework Core, see the limitations
            // here: https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations
            // To work around this, when the Sqlite database provider is used, all model properties of type DateTimeOffset
            // use the DateTimeOffsetToBinaryConverter
            // Based on: https://github.com/aspnet/EntityFrameworkCore/issues/10784#issuecomment-415769754
            // This only supports millisecond precision, but should be sufficient for most use cases.
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.ClrType.GetProperties())
                {
                    if (property.PropertyType == typeof(DateTimeOffset) || property.PropertyType == typeof(DateTimeOffset?))
                        modelBuilder
                           .Entity(entityType.Name)
                           .Property(property.Name)
                           .HasConversion(new DateTimeOffsetToBinaryConverter());

                    if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                        modelBuilder
                           .Entity(entityType.Name)
                           .Property(property.Name)
                           .HasConversion(new DateTimeToBinaryConverter());
                }
            }
    }
}