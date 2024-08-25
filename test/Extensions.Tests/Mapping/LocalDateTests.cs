using System.Reflection;
using AutoMapper;
using NodaTime;
using Rocket.Surgery.Extensions.Testing;

namespace Extensions.Tests.Mapping;

public partial class LocalDateTests(ITestOutputHelper testOutputHelper) : AutoFakeTest(testOutputHelper)
{

    [Fact]
    public void MapsFrom()
    {

        var foo = new Foo1
        {
            Bar = LocalDate.FromDateTime(DateTime.Now),
        };

        var result = Mapper.Map(foo).Bar;
        result.Should().Be(foo.Bar.ToDateTimeUnspecified());
    }

    [Fact]
    public void MapsTo()
    {

        var foo = new Foo3
        {
            Bar = DateTime.Now,
        };

        var result = Mapper.Map(foo).Bar;
        result.Should().Be(LocalDate.FromDateTime(foo.Bar));
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
    }

    private class Foo1
    {
        public LocalDate Bar { get; set; }
    }

    private class Foo3
    {
        public DateTime Bar { get; set; }
    }

    public class Converters : TypeConverterFactory
    {
        public override IEnumerable<Type> GetTypeConverters()
        {
            yield return typeof(ITypeConverter<LocalDate, DateTime>);
            yield return typeof(ITypeConverter<LocalDate?, DateTime?>);
            yield return typeof(ITypeConverter<DateTime, LocalDate>);
            yield return typeof(ITypeConverter<DateTime?, LocalDate?>);
            yield return typeof(ITypeConverter<LocalDate, DateOnly>);
            yield return typeof(ITypeConverter<LocalDate?, DateOnly?>);
            yield return typeof(ITypeConverter<DateOnly, LocalDate>);
            yield return typeof(ITypeConverter<DateOnly?, LocalDate?>);
        }
    }
}
