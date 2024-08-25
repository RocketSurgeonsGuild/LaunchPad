using System.Reflection;
using AutoMapper;
using NodaTime;
using Rocket.Surgery.Extensions.Testing;

namespace Extensions.Tests.Mapping;

public partial class OffsetDateTimeTests(ITestOutputHelper testOutputHelper) : AutoFakeTest(testOutputHelper)
{

    [Fact]
    public void MapsFrom()
    {

        var foo = new Foo1
        {
            Bar = OffsetDateTime.FromDateTimeOffset(DateTimeOffset.Now),
        };

        var result = Mapper.Map(foo).Bar;
        result.Should().Be(foo.Bar.ToDateTimeOffset());
    }

    [Fact]
    public void MapsTo()
    {

        var foo = new Foo3
        {
            Bar = DateTimeOffset.Now,
        };

        var result = Mapper.Map(foo).Bar;
        result.Should().Be(OffsetDateTime.FromDateTimeOffset(foo.Bar));
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
        public OffsetDateTime Bar { get; set; }
    }

    private class Foo3
    {
        public DateTimeOffset Bar { get; set; }
    }

    public class Converters : TypeConverterFactory
    {
        public override IEnumerable<Type> GetTypeConverters()
        {
            yield return typeof(ITypeConverter<OffsetDateTime, DateTimeOffset>);
            yield return typeof(ITypeConverter<OffsetDateTime?, DateTimeOffset?>);
            yield return typeof(ITypeConverter<DateTimeOffset, OffsetDateTime>);
            yield return typeof(ITypeConverter<DateTimeOffset?, OffsetDateTime?>);
        }
    }
}
