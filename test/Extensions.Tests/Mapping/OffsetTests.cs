using AutoMapper;
using NodaTime;
using Rocket.Surgery.Extensions.Testing;

namespace Extensions.Tests.Mapping;

public partial class OffsetTests(ITestOutputHelper testOutputHelper) : AutoFakeTest(testOutputHelper)
{

    [Fact]
    public void MapsFrom()
    {

        var foo = new Foo1
        {
            Bar = Offset.FromHours(11),
        };

        var result = Mapper.Map(foo).Bar;
        result.Should().Be(foo.Bar.ToTimeSpan());
    }

    [Fact]
    public void MapsTo()
    {

        var foo = new Foo3
        {
            Bar = TimeSpan.FromHours(10),
        };

        var result = Mapper.Map(foo).Bar;
        result.Should().Be(Offset.FromTimeSpan(foo.Bar));
    }

    protected override void Configure(IMapperConfigurationExpression expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        expression.CreateMap<Foo1, Foo3>().ReverseMap();
    }

    private class Foo1
    {
        public Offset Bar { get; set; }
    }

    private class Foo3
    {
        public TimeSpan Bar { get; set; }
    }

    public class Converters : TypeConverterFactory
    {
        public override IEnumerable<Type> GetTypeConverters()
        {
            yield return typeof(ITypeConverter<Offset, TimeSpan>);
            yield return typeof(ITypeConverter<Offset?, TimeSpan?>);
            yield return typeof(ITypeConverter<TimeSpan, Offset>);
            yield return typeof(ITypeConverter<TimeSpan?, Offset?>);
        }
    }
}
