using Microsoft.Extensions.Time.Testing;
using NodaTime;
using Riok.Mapperly.Abstractions;
using Rocket.Surgery.LaunchPad.Mapping;
using NodaTimeMapper = Rocket.Surgery.LaunchPad.Mapping.NodaTimeMapper;

namespace Extensions.Tests.Mapping;

public partial class OffsetDateTimeTests(ITestOutputHelper testOutputHelper) : MapperTestBase(testOutputHelper)
{
    private FakeTimeProvider _fakeTimeProvider = new();

    [Theory]
    [MapperData<Mapper>]
    public Task Maps_All_Methods(MethodResult result)
    {
        return VerifyMethod(
                   result,
                   new Mapper(),
                   _fakeTimeProvider.GetLocalNow(),
                   OffsetDateTime.FromDateTimeOffset(_fakeTimeProvider.GetLocalNow())
               )
              .UseParameters(result.ToString())
              .HashParameters();
    }

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
        public OffsetDateTime Bar { get; set; }
    }

    private class Foo2
    {
        public OffsetDateTime? Bar { get; set; }
    }

    private class Foo3
    {
        public DateTimeOffset Bar { get; set; }
    }

    private class Foo4
    {
        public DateTimeOffset? Bar { get; set; }
    }
}
