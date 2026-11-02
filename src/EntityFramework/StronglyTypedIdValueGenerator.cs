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
    ///     Create a value generator for the given type, in the shape <c>PropertyBuilder.HasValueGenerator</c> takes.
    /// </summary>
    /// <param name="factory">Creates each new id.</param>
    /// <typeparam name="T">The strongly typed id.</typeparam>
    /// <returns>The value generator factory.</returns>
    public static Func<IReadOnlyProperty, ITypeBase, ValueGenerator> Create<T>(Func<T> factory) where T : struct => (_, _) => new StronglyTypedIdValueGenerator<T>(factory);
}
