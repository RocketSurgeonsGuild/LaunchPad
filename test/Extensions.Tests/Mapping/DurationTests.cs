using AutoMapper;
using FluentAssertions;
using JetBrains.Annotations;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CA1034 // Nested types should not be visible

namespace Extensions.Tests.Mapping
{
    public class DurationTests : TypeConverterTest<DurationTests.Converters>
    {
        public DurationTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void ValidateMapping() => Config.AssertConfigurationIsValid();

        [Fact]
        public void CanConvertDurationToMinutes()
        {
            var foo = new Foo1 { Bar = Duration.FromMinutes(300) };

            var o = Mapper.Map<Foo7>(foo);

            Assert.Equal(18000, o.Bar);
        }

        [Fact]
        public void CanConvertMinutesToDuration()
        {
            var foo = new Foo7 { Bar = 300 };

            var o = Mapper.Map<Foo1>(foo);

            Assert.Equal(Duration.FromMinutes(5), o.Bar);
        }

        [Fact]
        public void MapsFrom_TimeSpan()
        {
            var mapper = Config.CreateMapper();

            var foo = new Foo1
            {
                Bar = Duration.FromDays(1)
            };

            var result = mapper.Map<Foo3>(foo).Bar;
            result.Should().Be(foo.Bar.ToTimeSpan());
        }

        [Fact]
        public void MapsTo_TimeSpan()
        {
            var mapper = Config.CreateMapper();

            var foo = new Foo3
            {
                Bar = TimeSpan.FromDays(1)
            };

            var result = mapper.Map<Foo1>(foo).Bar;
            result.Should().Be(Duration.FromTimeSpan(foo.Bar));
        }

        [Fact]
        public void MapsFrom_Int64()
        {
            var mapper = Config.CreateMapper();

            var foo = new Foo1
            {
                Bar = Duration.FromDays(1)
            };

            var result = mapper.Map<Foo5>(foo).Bar;
            result.Should().Be(Convert.ToInt64(foo.Bar.BclCompatibleTicks / NodaConstants.TicksPerMillisecond));
        }

        [Fact]
        public void MapsTo_Int64()
        {
            var mapper = Config.CreateMapper();

            var foo = new Foo5
            {
                Bar = 10000L
            };

            var result = mapper.Map<Foo1>(foo).Bar;
            result.Should().Be(Duration.FromTicks(foo.Bar * NodaConstants.TicksPerMillisecond));
        }

        [Fact]
        public void MapsFrom_Int32()
        {
            var mapper = Config.CreateMapper();

            var foo = new Foo1
            {
                Bar = Duration.FromDays(1)
            };

            var result = mapper.Map<Foo7>(foo).Bar;
            result.Should().Be(Convert.ToInt32(foo.Bar.BclCompatibleTicks / NodaConstants.TicksPerSecond));
        }

        [Fact]
        public void MapsTo_Int32()
        {
            var mapper = Config.CreateMapper();

            var foo = new Foo7
            {
                Bar = 10000
            };

            var result = mapper.Map<Foo1>(foo).Bar;
            result.Should().Be(Duration.FromTicks(foo.Bar * NodaConstants.TicksPerSecond));
        }

        [Fact]
        public void MapsFrom_Double()
        {
            var mapper = Config.CreateMapper();

            var foo = new Foo1
            {
                Bar = Duration.FromDays(1)
            };

            var result = mapper.Map<Foo8>(foo).Bar;
            result.Should().Be(Convert.ToInt32(foo.Bar.BclCompatibleTicks / NodaConstants.TicksPerMillisecond));
        }

        [Fact]
        public void MapsTo_Double()
        {
            var mapper = Config.CreateMapper();

            var foo = new Foo8
            {
                Bar = 10000.1256d
            };

            var result = mapper.Map<Foo1>(foo).Bar;
            result.Should().Be(Duration.FromTicks(foo.Bar * NodaConstants.TicksPerMillisecond));
        }

        [Fact]
        public void MapsFrom_Decimal()
        {
            var mapper = Config.CreateMapper();

            var foo = new Foo1
            {
                Bar = Duration.FromDays(1)
            };

            var result = mapper.Map<Foo9>(foo).Bar;
            result.Should().Be(Convert.ToInt32(foo.Bar.BclCompatibleTicks / NodaConstants.TicksPerMillisecond));
        }

        [Fact]
        public void MapsTo_Decimal()
        {
            var mapper = Config.CreateMapper();

            var foo = new Foo9
            {
                Bar = 10000.125M
            };

            var result = mapper.Map<Foo1>(foo).Bar;
            result.Should().Be(Duration.FromTicks((double)foo.Bar * NodaConstants.TicksPerMillisecond));
        }

        [Theory]
        [ClassData(typeof(TypeConverterData<Converters>))]
        public void AutomatedTests(Type source, Type destination, object? sourceValue)
        {
            var method = typeof(IMapperBase).GetMethods(BindingFlags.Public | BindingFlags.Instance)
               .First(
                    x => x.ContainsGenericParameters && x.IsGenericMethodDefinition &&
                        x.GetGenericMethodDefinition().GetGenericArguments().Length == 2 &&
                        x.GetParameters().Length == 1
                );
            var result = method.MakeGenericMethod(source, destination).Invoke(Mapper, new[] { sourceValue });

            if (sourceValue == null)
            {
                result.Should().BeNull();
            }
            else
            {
                result.Should().BeOfType(Nullable.GetUnderlyingType(destination) ?? destination).And.NotBeNull();
            }
        }

        protected override void Configure([NotNull] IMapperConfigurationExpression x)
        {
            if (x == null)
            {
                throw new ArgumentNullException(nameof(x));
            }

            x.CreateMap<Foo1, Foo3>().ReverseMap();
            x.CreateMap<Foo1, Foo5>().ReverseMap();
            x.CreateMap<Foo1, Foo7>().ReverseMap();
            x.CreateMap<Foo1, Foo8>().ReverseMap();
            x.CreateMap<Foo1, Foo9>().ReverseMap();
        }

        public class Foo1
        {
            public Duration Bar { get; set; }
        }

        public class Foo3
        {
            public TimeSpan Bar { get; set; }
        }

        public class Foo5
        {
            public long Bar { get; set; }
        }

        public class Foo7
        {
            public int Bar { get; set; }
        }

        public class Foo8
        {
            public double Bar { get; set; }
        }

        public class Foo9
        {
            public decimal Bar { get; set; }
        }

        public class Converters : TypeConverterFactory
        {
            public override IEnumerable<Type> GetTypeConverters()
            {
                yield return typeof(ITypeConverter<Duration, TimeSpan>);
                yield return typeof(ITypeConverter<Duration?, TimeSpan?>);
                yield return typeof(ITypeConverter<TimeSpan, Duration>);
                yield return typeof(ITypeConverter<TimeSpan?, Duration?>);
                yield return typeof(ITypeConverter<Duration, long>);
                yield return typeof(ITypeConverter<Duration?, long?>);
                yield return typeof(ITypeConverter<long, Duration>);
                yield return typeof(ITypeConverter<long?, Duration?>);
                yield return typeof(ITypeConverter<Duration, int>);
                yield return typeof(ITypeConverter<Duration?, int?>);
                yield return typeof(ITypeConverter<int, Duration>);
                yield return typeof(ITypeConverter<int?, Duration?>);
                yield return typeof(ITypeConverter<Duration, double>);
                yield return typeof(ITypeConverter<Duration?, double?>);
                yield return typeof(ITypeConverter<double, Duration>);
                yield return typeof(ITypeConverter<double?, Duration?>);
                yield return typeof(ITypeConverter<Duration, decimal>);
                yield return typeof(ITypeConverter<Duration?, decimal?>);
                yield return typeof(ITypeConverter<decimal, Duration>);
                yield return typeof(ITypeConverter<decimal?, Duration?>);
            }
        }
    }
}