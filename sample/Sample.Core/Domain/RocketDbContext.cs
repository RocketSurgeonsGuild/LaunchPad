using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Rocket.Surgery.LaunchPad.EntityFramework;
using Sample.Core.Models;

namespace Sample.Core.Domain;

public class RocketDbContext : LpContext<RocketDbContext>
{
    public RocketDbContext(DbContextOptions<RocketDbContext>? options = null) : base(options ?? new DbContextOptions<RocketDbContext>())
    {
    }

    public DbSet<ReadyRocket> Rockets { get; set; } = null!;
    public DbSet<LaunchRecord> LaunchRecords { get; set; } = null!;

#if NET6_0_OR_GREATER
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
           .Properties<LaunchRecordId>()
           .HaveConversion<LaunchRecordId.EfCoreValueConverter>();
        configurationBuilder
           .Properties<RocketId>()
           .HaveConversion<RocketId.EfCoreValueConverter>();
    }
#endif
}

public class StronglyTypedIdValueConverterSelector : ValueConverterSelector
{
    [return: NotNullIfNotNull("type")]
    private static Type? UnwrapNullableType(Type? type)
    {
        if (type is null)
        {
            return null;
        }

        return Nullable.GetUnderlyingType(type) ?? type;
    }

    // The dictionary in the base type is private, so we need our own one here.
    private readonly ConcurrentDictionary<(Type ModelClrType, Type ProviderClrType), ValueConverterInfo> _converters
        = new ConcurrentDictionary<(Type ModelClrType, Type ProviderClrType), ValueConverterInfo>();

    public StronglyTypedIdValueConverterSelector(ValueConverterSelectorDependencies dependencies) : base(dependencies)
    {
    }

    public override IEnumerable<ValueConverterInfo> Select(Type modelClrType, Type? providerClrType = null)
    {
        var baseConverters = base.Select(modelClrType, providerClrType);
        foreach (var converter in baseConverters)
        {
            yield return converter;
        }

        // Extract the "real" type T from Nullable<T> if required
        var underlyingModelType = UnwrapNullableType(modelClrType);
        var underlyingProviderType = UnwrapNullableType(providerClrType);

        // 'null' means 'get any value converters for the modelClrType'
        if (underlyingProviderType is null || underlyingProviderType == typeof(Guid))
        {
            // Try and get a nested class with the expected name.
            var converterType = underlyingModelType.GetNestedType("EfCoreValueConverter");

            if (converterType != null)
            {
                yield return _converters.GetOrAdd(
                    ( underlyingModelType, typeof(Guid) ),
                    _ =>
                    {
                        // Create an instance of the converter whenever it's requested.
                        Func<ValueConverterInfo, ValueConverter> factory =
                            info => (ValueConverter)Activator.CreateInstance(converterType, info.MappingHints)!;

                        // Build the info for our strongly-typed ID => Guid converter
                        return new ValueConverterInfo(modelClrType, typeof(Guid), factory);
                    }
                );
            }
        }
    }
}
