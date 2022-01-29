using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Bogus;
using NodaTime;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.Mapping;
using Rocket.Surgery.LaunchPad.Mapping.Profiles;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CA1000 // Do not declare static members on generic types

namespace Extensions.Tests.Mapping;

public abstract class TypeConverterFactory
{
    public abstract IEnumerable<Type> GetTypeConverters();
}

internal class TypeConverterData<T> : TypeConverterData
    where T : TypeConverterFactory, new()
{
    public TypeConverterData() : base(new T().GetTypeConverters)
    {
    }
}

internal abstract class TypeConverterData : TheoryData<Type, Type, object?>
{
    internal static IEnumerable<(Type source, Type destination)> GetValueTypePairs(IEnumerable<Type> typeConverters)
    {
        return typeConverters
              .Select(
                   x => (
                       source: Nullable.GetUnderlyingType(x.GetGenericArguments()[0]) ?? x.GetGenericArguments()[0],
                       destination: Nullable.GetUnderlyingType(x.GetGenericArguments()[1]) ?? x.GetGenericArguments()[1] )
               )
              .Where(x => x.source.IsValueType && x.destination.IsValueType)
              .Distinct();
    }

    private static readonly Faker Faker = new();


    private static object GetRandomValue(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        if (type == typeof(int))
        {
            return Faker.Random.Int();
        }

        if (type == typeof(long))
        {
            return Faker.Random.Long();
        }

        if (type == typeof(short))
        {
            return Faker.Random.Short();
        }

        if (type == typeof(float))
        {
            return Faker.Random.Float();
        }

        if (type == typeof(double))
        {
            return Faker.Random.Double();
        }

        if (type == typeof(decimal))
        {
            return Faker.Random.Decimal();
        }

        if (type == typeof(Duration))
        {
            return Duration.FromTimeSpan(Faker.Date.Timespan());
        }

        if (type == typeof(TimeSpan))
        {
            return Faker.Date.Timespan(TimeSpan.FromDays(1));
        }

        if (type == typeof(Instant))
        {
            return Instant.FromDateTimeOffset(Faker.Date.RecentOffset());
        }

        if (type == typeof(LocalDateTime))
        {
            return LocalDateTime.FromDateTime(Faker.Date.Recent());
        }

        if (type == typeof(OffsetDateTime))
        {
            return OffsetDateTime.FromDateTimeOffset(Faker.Date.RecentOffset());
        }

        if (type == typeof(LocalTime))
        {
            return LocalTime.FromTicksSinceMidnight(Faker.Date.Timespan(TimeSpan.FromDays(1)).Ticks);
        }

        if (type == typeof(LocalDate))
        {
            return LocalDate.FromDateTime(Faker.Date.Recent());
        }

        if (type == typeof(Offset))
        {
            return Offset.FromTimeSpan(Faker.Date.Timespan(TimeSpan.FromHours(12)));
        }

        if (type == typeof(DateTime))
        {
            return Faker.Date.Recent();
        }

        if (type == typeof(DateTimeOffset))
        {
            return Faker.Date.RecentOffset();
        }

        throw new NotSupportedException($"type {type.FullName} is not supported");
    }

    protected TypeConverterData(Func<IEnumerable<Type>> typeConverterFactory)
    {
        static (Type source, Type sourceClass, Type destination, Type destinationClass) GetWrappedClasses((Type source, Type destination) item)
        {
            var (source, destination) = item;
            var sourceFoo = typeof(Foo<>).MakeGenericType(source);
            var destinationFoo = typeof(Foo<>).MakeGenericType(destination);
            return ( source, sourceFoo, destination, destinationFoo );
        }

        static object CreateValue(Type type, object value)
        {
            return typeof(Foo).GetMethod(nameof(Foo.Create))!?.MakeGenericMethod(type).Invoke(null, new[] { value })!;
        }

        foreach (var (source, sourceClass, destination, destinationClass) in GetValueTypePairs(typeConverterFactory())
                                                                            .SelectMany(
                                                                                 item => new[]
                                                                                 {
                                                                                     item,
                                                                                     ( typeof(Nullable<>).MakeGenericType(item.source),
                                                                                       typeof(Nullable<>).MakeGenericType(item.destination) ),
                                                                                     ( item.source, typeof(Nullable<>).MakeGenericType(item.destination) )
                                                                                 }
                                                                             ).Select(GetWrappedClasses))
        {
            var sourceValue = CreateValue(source, GetRandomValue(source));
            Add(sourceClass, destinationClass, sourceValue);

            if (Nullable.GetUnderlyingType(source) == null)
            {
                continue;
            }

            foreach (var item in Faker.Make(3, () => CreateValue(source, GetRandomValue(source).OrNull(Faker))))
            {
                Add(sourceClass, destinationClass, item);
            }
        }
    }
}

public abstract class TypeConverterTest : AutoFakeTest
{
    protected TypeConverterTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper, LogEventLevel.Debug)
    {
        Config = new MapperConfiguration(
            x =>
            {
                x.AddProfile<NodaTimeProfile>();
                Configure(x);
            }
        );
        Mapper = Config.CreateMapper();
    }

    protected IMapper Mapper { get; }
    protected AutoMapperOptions Options { get; } = new AutoMapperOptions();
    protected MapperConfiguration Config { get; }
    protected abstract void Configure(IMapperConfigurationExpression expression);
}

public abstract class TypeConverterTest<T> : AutoFakeTest
    where T : TypeConverterFactory, new()
{
    // TODO: Refactor this to forward parent class constructor values
    protected TypeConverterTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper, LogEventLevel.Debug)
    {
        Config = new MapperConfiguration(
            x =>
            {
                x.AddProfile<NodaTimeProfile>();
                foreach (var (source, destination) in TypeConverterData.GetValueTypePairs(new T().GetTypeConverters()).SelectMany(
                             item => new[]
                             {
                                 item,
                                 ( typeof(Nullable<>).MakeGenericType(item.source),
                                   typeof(Nullable<>).MakeGenericType(item.destination) ),
                                 ( item.source, typeof(Nullable<>).MakeGenericType(item.destination) )
                             }
                         ))
                {
                    x.CreateMap(typeof(Foo<>).MakeGenericType(source), typeof(Foo<>).MakeGenericType(destination));
                }

                Configure(x);
            }
        );
        Mapper = Config.CreateMapper();
    }

    protected IMapper Mapper { get; }
    protected AutoMapperOptions Options { get; } = new AutoMapperOptions();
    protected MapperConfiguration Config { get; }

    protected abstract void Configure(IMapperConfigurationExpression expression);
}
