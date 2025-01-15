using Microsoft.Extensions.Time.Testing;

using NodaTime;

using Riok.Mapperly.Abstractions;

using Rocket.Surgery.LaunchPad.Mapping;
using NodaTimeMapper = Rocket.Surgery.LaunchPad.Mapping.NodaTimeMapper;

namespace Extensions.Tests.Mapping;

public partial class OffsetTests(ITestOutputHelper testOutputHelper) : MapperTestBase(testOutputHelper)
{
    [Theory]
    [MapperData<Mapper>]
    public Task Maps_All_Methods(MethodResult result) => VerifyMethod(
        result,
        new Mapper(),
        Offset.FromHours(11),
        TimeSpan.FromHours(10)
        )
        .UseParameters(result.ToString())
        .HashParameters();

    private readonly FakeTimeProvider _fakeTimeProvider = new();

    [Mapper]
    [UseStaticMapper(typeof(NodaTimeMapper))]
    [UseStaticMapper(typeof(NodaTimeDateTimeMapper))]
    private partial class Mapper
    {
        public partial Foo1 MapFoo1(Foo2 foo);
        public partial Foo1 MapFoo1(Foo3 foo);
        public partial Foo1 MapFoo1(Foo4 foo);

        public partial Foo2 MapFoo2(Foo1 foo);
        public partial Foo2 MapFoo2(Foo3 foo);
        public partial Foo2 MapFoo2(Foo4 foo);

        public partial Foo3 MapFoo3(Foo1 foo);
        public partial Foo3 MapFoo3(Foo2 foo);
        public partial Foo3 MapFoo3(Foo4 foo);

        public partial Foo4 MapFoo4(Foo1 foo);
        public partial Foo4 MapFoo4(Foo2 foo);
        public partial Foo4 MapFoo4(Foo3 foo);
    }

    private class Foo1
    {
        public Offset Bar { get; set; }
    }

    private class Foo2
    {
        public Offset? Bar { get; set; }
    }

    private class Foo3
    {
        public TimeSpan Bar { get; set; }
    }

    private class Foo4
    {
        public TimeSpan? Bar { get; set; }
    }
}
