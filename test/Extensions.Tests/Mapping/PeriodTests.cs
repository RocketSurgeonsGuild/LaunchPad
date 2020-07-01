using System;
using AutoMapper;
using FluentAssertions;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Text;
using Rocket.Surgery.Extensions.AutoMapper.Converters;
using Xunit;
using Xunit.Abstractions;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Rocket.Surgery.Extensions.AutoMapper.Tests
{
    public class PeriodTests : TypeConverterTest<PeriodConverter>
    {
        public PeriodTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void ValidateMapping() => Config.AssertConfigurationIsValid();

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
            result.Should().Be(PeriodPattern.Roundtrip.Parse(foo.Bar).Value);
        }

        protected override void Configure(IMapperConfigurationExpression x)
        {
            if (x == null)
            {
                throw new ArgumentNullException(nameof(x));
            }

            x.CreateMap<Foo1, Foo3>().ReverseMap();
        }

        private class Foo1
        {
            public Period? Bar { get; set; }
        }

        private class Foo3
        {
            public string? Bar { get; set; }
        }
    }
}