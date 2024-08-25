using System.Reflection;
using NodaTime;
using Riok.Mapperly.Abstractions;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.Mapping.Profiles;

#pragma warning disable CA1034 // Nested types should not be visible

namespace Extensions.Tests.Mapping;

public partial class DurationTests(ITestOutputHelper testOutputHelper) : AutoFakeTest(testOutputHelper)
{
    [Fact]
    public void MapsFrom_TimeSpan()
    {
        var foo = new Foo1
        {
            Bar = Duration.FromDays(1),
        };

        var result = Mapper.Map(foo).Bar;
        result.Should().Be(foo.Bar.ToTimeSpan());
    }

    [Fact]
    public void MapsTo_TimeSpan()
    {
        var foo = new Foo3
        {
            Bar = TimeSpan.FromDays(1),
        };

        var result = Mapper.Map(foo).Bar;
        result.Should().Be(Duration.FromTimeSpan(foo.Bar));
    }

    public class Foo1
    {
        public Duration Bar { get; set; }
    }

    public class Foo3
    {
        public TimeSpan Bar { get; set; }
    }

    [Mapper]
    [UseStaticMapper(typeof(NodaTimeMapper))]
    public static partial class Mapper
    {
        public static partial Foo1 Map(Foo3 source);
        public static partial Foo3 Map(Foo1 source);
    }
}
