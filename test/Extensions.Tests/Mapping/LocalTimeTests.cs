using System.Reflection;
using AutoMapper;
using NodaTime;
using NodaTime.Extensions;

namespace Extensions.Tests.Mapping;

public class LocalTimeTests(ITestOutputHelper testOutputHelper) : TypeConverterTest<LocalTimeTests.Converters>(testOutputHelper)
{
    [Fact]
    public void ValidateMapping()
    {
        Config.AssertConfigurationIsValid();
    }

    [Fact]
    public void MapsFrom_DateTime()
    {
        var mapper = Config.CreateMapper();

        var foo = new Foo1
        {
            Bar = LocalTime.FromTicksSinceMidnight(10000),
        };

        var result = mapper.Map<Foo3>(foo).Bar;
        result.Should().Be(new(foo.Bar.TickOfDay));
    }

    [Fact]
    public void MapsTo_DateTime()
    {
        var mapper = Config.CreateMapper();

        var foo = new Foo3
        {
            Bar = TimeSpan.FromMinutes(502),
        };

        var result = mapper.Map<Foo1>(foo).Bar;
        result.Should().Be(new(502 / 60, 502 % 60));
    }

    [Fact]
    public void MapsFrom_DateTimeOffset()
    {
        var mapper = Config.CreateMapper();

        var foo = new Foo1
        {
            Bar = LocalTime.FromTicksSinceMidnight(10000),
        };

        var result = mapper.Map<Foo5>(foo).Bar;
        result.Should().Be(foo.Bar.ToTimeOnly());
    }

    [Fact]
    public void MapsTo_DateTimeOffset()
    {
        var mapper = Config.CreateMapper();

        var foo = new Foo5
        {
            Bar = TimeOnly.FromDateTime(DateTime.Now),
        };

        var result = mapper.Map<Foo1>(foo).Bar;
        result.Should().Be(foo.Bar.ToLocalTime());
    }

    [Theory]
    [ClassData(typeof(TypeConverterData<Converters>))]
    public void AutomatedTests(Type source, Type destination, object? sourceValue)
    {
        var method = typeof(IMapperBase)
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .First(
                         x => x.ContainsGenericParameters
                          && x.IsGenericMethodDefinition
                          && x.GetGenericMethodDefinition().GetGenericArguments().Length == 2
                          && x.GetParameters().Length == 1
                     );
        var result = method.MakeGenericMethod(source, destination).Invoke(Mapper, new[] { sourceValue, });

        if (sourceValue == null)
            result.Should().BeNull();
        else
            result.Should().BeOfType(Nullable.GetUnderlyingType(destination) ?? destination).And.NotBeNull();
    }

    protected override void Configure(IMapperConfigurationExpression expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        expression.CreateMap<Foo1, Foo3>().ReverseMap();
        expression.CreateMap<Foo1, Foo5>().ReverseMap();
    }

    private class Foo1
    {
        public LocalTime Bar { get; set; }
    }

    private class Foo3
    {
        public TimeSpan Bar { get; set; }
    }

    private class Foo5
    {
        public TimeOnly Bar { get; set; }
    }

    public class Converters : TypeConverterFactory
    {
        public override IEnumerable<Type> GetTypeConverters()
        {
            yield return typeof(ITypeConverter<LocalTime, TimeSpan>);
            yield return typeof(ITypeConverter<LocalTime?, TimeSpan?>);
            yield return typeof(ITypeConverter<TimeSpan, LocalTime>);
            yield return typeof(ITypeConverter<TimeSpan?, LocalTime?>);
            yield return typeof(ITypeConverter<LocalTime, TimeOnly>);
            yield return typeof(ITypeConverter<LocalTime?, TimeOnly?>);
            yield return typeof(ITypeConverter<TimeOnly, LocalTime>);
            yield return typeof(ITypeConverter<TimeOnly?, LocalTime?>);
        }
    }
}