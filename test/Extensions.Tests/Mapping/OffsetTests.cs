using System;
using AutoMapper;
using FluentAssertions;
using NodaTime;
using Rocket.Surgery.Extensions.AutoMapper.Converters;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Extensions.AutoMapper.Tests
{
    public class OffsetTests : TypeConverterTest<OffsetConverter>
    {
        public OffsetTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void ValidateMapping() => Config.AssertConfigurationIsValid();

        [Fact]
        public void MapsFrom()
        {
            var mapper = Config.CreateMapper();

            var foo = new Foo1
            {
                Bar = Offset.FromHours(11)
            };

            var result = mapper.Map<Foo3>(foo).Bar;
            result.Should().Be(foo.Bar.ToTimeSpan());
        }

        [Fact]
        public void MapsTo()
        {
            var mapper = Config.CreateMapper();

            var foo = new Foo3
            {
                Bar = TimeSpan.FromHours(10)
            };

            var result = mapper.Map<Foo1>(foo).Bar;
            result.Should().Be(Offset.FromTimeSpan(foo.Bar));
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
            public Offset Bar { get; set; }
        }

        private class Foo3
        {
            public TimeSpan Bar { get; set; }
        }
    }
}