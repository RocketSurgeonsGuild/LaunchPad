using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.Time.Testing;
using NodaTime;
using Riok.Mapperly.Abstractions;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.Mapping;
using Rocket.Surgery.LaunchPad.Mapping.Profiles;

namespace Extensions.Tests.Mapping;

public partial class InstantTests(ITestOutputHelper testOutputHelper) : AutoFakeTest(testOutputHelper)
{
    FakeTimeProvider _fakeTimeProvider = new ();

    [Fact]
    public void MapsFrom_DateTime()
    {

        var foo = new Foo1
        {
            Bar = Instant.FromDateTimeOffset(_fakeTimeProvider.GetUtcNow()),
        };

        var result = Mapper.Map(foo).Bar;
        result.Should().Be(_fakeTimeProvider.GetUtcNow());
    }

    [Fact]
    public void MapsTo_DateTime()
    {

        var foo = new Foo3
        {
            Bar = DateTime.UtcNow,
        };

        var result = Mapper.Map(foo).Bar;
        result.Should().Be(Instant.FromDateTimeUtc(foo.Bar));
    }

    [Fact]
    public void MapsFrom_DateTimeOffset()
    {

        var foo = new Foo1
        {
            Bar = Instant.FromDateTimeOffset(DateTimeOffset.Now),
        };

        var result = Mapper.Map(foo).Bar;
        result.Should().Be(foo.Bar.ToDateTimeOffset());
    }

    [Fact]
    public void MapsTo_DateTimeOffset()
    {

        var foo = new Foo5
        {
            Bar = DateTimeOffset.Now,
        };

        var result = Mapper.Map(foo).Bar;
        result.Should().Be(Instant.FromDateTimeOffset(foo.Bar));
    }

    private class Foo1
    {
        public Instant Bar { get; set; }
    }

    private class Foo2
    {
        public Instant? Bar { get; set; }
    }

    private class Foo3
    {
        public DateTime Bar { get; set; }
    }

    private class Foo4
    {
        public DateTime? Bar { get; set; }
    }

    private class Foo5
    {
        public DateTimeOffset Bar { get; set; }
    }

    private class Foo6
    {
        public DateTimeOffset? Bar { get; set; }
    }

    public record MethodResult(MethodInfo MethodInfo, string Name, Type Source, Type Destination)
    {
        public override string ToString()
        {
            return $"{Name}({Source.Name} -> {Destination.Name})";
        }
    }

    class A : CombinatorialMemberDataAttribute
    {

    }

    [Mapper, PublicAPI]
    [UseStaticMapper(typeof(NodaTimeMapper))]
    [UseStaticMapper(typeof(NodaTimeDateTimeMapper))]
    private static partial class Mapper
    {
        public static partial Foo1 MapFoo1(Foo2 source);
        public static partial Foo1 MapFoo1(Foo3 source);
        public static partial Foo1 MapFoo1(Foo4 source);
        public static partial Foo1 MapFoo1(Foo5 source);
        public static partial Foo1 MapFoo1(Foo6 source);

        public static partial Foo2 MapFoo2(Foo1 source);
        public static partial Foo2 MapFoo2(Foo3 source);
        public static partial Foo2 MapFoo2(Foo4 source);
        public static partial Foo2 MapFoo2(Foo5 source);
        public static partial Foo2 MapFoo2(Foo6 source);

        public static partial Foo3 MapFoo3(Foo1 source);
        public static partial Foo3 MapFoo3(Foo2 source);
        public static partial Foo3 MapFoo3(Foo4 source);
        public static partial Foo3 MapFoo3(Foo5 source);
        public static partial Foo3 MapFoo3(Foo6 source);

        public static partial Foo4 MapFoo4(Foo1 source);
        public static partial Foo4 MapFoo4(Foo2 source);
        public static partial Foo4 MapFoo4(Foo3 source);
        public static partial Foo4 MapFoo4(Foo5 source);
        public static partial Foo4 MapFoo4(Foo6 source);

        public static partial Foo5 MapFoo5(Foo1 source);
        public static partial Foo5 MapFoo5(Foo2 source);
        public static partial Foo5 MapFoo5(Foo3 source);
        public static partial Foo5 MapFoo5(Foo4 source);
        public static partial Foo5 MapFoo5(Foo6 source);

        public static partial Foo6 MapFoo6(Foo1 source);
        public static partial Foo6 MapFoo6(Foo2 source);
        public static partial Foo6 MapFoo6(Foo3 source);
        public static partial Foo6 MapFoo6(Foo4 source);
        public static partial Foo6 MapFoo6(Foo5 source);
    }
}
