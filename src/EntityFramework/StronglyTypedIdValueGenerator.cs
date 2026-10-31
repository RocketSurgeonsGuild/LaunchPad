using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Rocket.Surgery.LaunchPad.EntityFramework;

/// <summary>
///     Value generator for StronglyTypedIds
/// </summary>
/// <typeparam name="T"></typeparam>
public class StronglyTypedIdValueGenerator<T> : ValueGenerator<T>
{
    private readonly Func<T> _factory;

    internal StronglyTypedIdValueGenerator(Func<T> factory) => _factory = factory;

    /// <inheritdoc />
    public override bool GeneratesTemporaryValues => false;

    /// <inheritdoc />
    public override T Next(EntityEntry entry) => _factory();
}

/// <summary>
///     Factory a StronglyTypedId value generator
/// </summary>
public static class StronglyTypedIdValueGenerator
{
    /// <summary>
    ///     Create a value generator for the given type
    /// </summary>
    /// <param name="factory"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Func<IProperty, IEntityType, ValueGenerator> Create<T>(Func<T> factory) where T : struct => (_, _) => new StronglyTypedIdValueGenerator<T>(factory);
}
