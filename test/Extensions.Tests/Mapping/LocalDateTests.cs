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

namespace Extensions.Tests.Mapping
{
    public class LocalDateTests : TypeConverterTest<LocalDateTests.Converters>
    {
        public LocalDateTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void ValidateMapping() => Config.AssertConfigurationIsValid();

        [Fact]
        public void MapsFrom()
        {
            var mapper = Config.CreateMapper();

            var foo = new Foo1
            {
                Bar = LocalDate.FromDateTime(DateTime.Now)
            };

            var result = mapper.Map<Foo3>(foo).Bar;
            result.Should().Be(foo.Bar.ToDateTimeUnspecified());
        }

        [Fact]
        public void MapsTo()
        {
            var mapper = Config.CreateMapper();

            var foo = new Foo3
            {
                Bar = DateTime.Now
            };

            var result = mapper.Map<Foo1>(foo).Bar;
            result.Should().Be(LocalDate.FromDateTime(foo.Bar));
        }

        [Theory]
        [ClassData(typeof(TypeConverterData<Converters>))]
        public void AutomatedTests(Type source, Type destination, object sourceValue)
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
            }
        }
    }
}