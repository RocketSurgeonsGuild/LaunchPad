using AutoMapper;
using FluentAssertions;
using NodaTime;
using NodaTime.Text;
using Xunit;
using Xunit.Abstractions;

namespace Extensions.Tests.Mapping;

public class PeriodTests : TypeConverterTest<PeriodTests.Converters>
{
    [Fact]
    public void ValidateMapping()
    {
        Config.AssertConfigurationIsValid();
    }

    [Fact]
    public void MapsFrom()
    {
        var mapper = Config.CreateMapper();

        var foo = new Foo1
        {
            Bar = Period.FromMonths(10)
        };

        var result = mapper.Map<Foo3>(foo).Bar;
        result.Should().Be("P10M");
    }

    [Fact]
    public void MapsTo()
    {
        var mapper = Config.CreateMapper();

        var foo = new Foo3
        {
            Bar = "P5M"
        };

        var result = mapper.Map<Foo1>(foo).Bar;
        result!.Should().Be(PeriodPattern.Roundtrip.Parse(foo.Bar).Value);
    }

    public PeriodTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    protected override void Configure(IMapperConfigurationExpression expression)
    {
        if (expression == null)
        {
            throw new ArgumentNullException(nameof(expression));
        }

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
