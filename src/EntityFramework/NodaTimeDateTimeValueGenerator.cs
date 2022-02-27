using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using NodaTime;

namespace Rocket.Surgery.LaunchPad.EntityFramework;

/// <summary>
///     A value generator for DateTimeOffset from the NodaTime <see cref="IClock" />
/// </summary>
[PublicAPI]
public class NodaTimeDateTimeValueGenerator : ValueGenerator<DateTime>
{
    private readonly IClock _clock;

    /// <summary>
    ///     Create the generator with the given clock
    /// </summary>
    /// <param name="clock"></param>
    public NodaTimeDateTimeValueGenerator(IClock clock)
    {
        _clock = clock;
    }

    /// <inheritdoc />
    public override bool GeneratesTemporaryValues { get; }

    /// <inheritdoc />
    public override DateTime Next(EntityEntry entry)
    {
        return _clock.GetCurrentInstant().ToDateTimeUtc();
    }
}

/// <summary>
///     Value generator for StronglyTypedIds
/// </summary>
/// <typeparam name="T"></typeparam>
public class StronglyTypedIdValueGenerator<T> : ValueGenerator<T>
{
    private readonly Func<T> _factory;

    internal StronglyTypedIdValueGenerator(Func<T> factory)
    {
        _factory = factory;
    }

    /// <inheritdoc />
    public override bool GeneratesTemporaryValues => false;

    /// <inheritdoc />
    public override T Next(EntityEntry entry)
    {
        return _factory();
    }
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
    public static Func<IProperty, IEntityType, ValueGenerator> Create<T>(Func<T> factory) where T : struct
    {
        return (_, _) => new StronglyTypedIdValueGenerator<T>(factory);
    }
}
