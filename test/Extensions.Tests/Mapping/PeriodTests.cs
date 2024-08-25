using AutoMapper;
using NodaTime;
using NodaTime.Text;
using Rocket.Surgery.Extensions.Testing;

namespace Extensions.Tests.Mapping;

public partial class PeriodTests(ITestOutputHelper testOutputHelper) : AutoFakeTest(testOutputHelper)
{

    [Fact]
    public void MapsFrom()
    {

        var foo = new Foo1
        {
            Bar = Period.FromMonths(10),
        };

        var result = Mapper.Map(foo).Bar;
        result.Should().Be("P10M");
    }

    [Fact]
    public void MapsTo()
    {

        var foo = new Foo3
        {
            Bar = "P5M",
        };

        var result = Mapper.Map(foo).Bar;
        result!.Should().Be(PeriodPattern.Roundtrip.Parse(foo.Bar).Value);
    }

    protected override void Configure(IMapperConfigurationExpression expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        expression.CreateMap<Foo1, Foo3>().ReverseMap();
    }

    private class Foo1
    {
        public Period? Bar { get; set; }
    }

    private class Foo3
    {
        public string? Bar { get; set; }
    }

    public class Converters : TypeConverterFactory
    {
        public override IEnumerable<Type> GetTypeConverters()
        {
            yield return typeof(ITypeConverter<Period, string>);
            yield return typeof(ITypeConverter<string, Period>);
        }
    }
}
