using Microsoft.Extensions.Time.Testing;

using NodaTime;

using Riok.Mapperly.Abstractions;

using Rocket.Surgery.LaunchPad.Mapping;
using NodaTimeMapper = Rocket.Surgery.LaunchPad.Mapping.NodaTimeMapper;

namespace Extensions.Tests.Mapping;

public partial class InstantTests(ITestContextAccessor testContext) : MapperTestBase(testContext)
{
    [Theory]
    [MapperData<Mapper>]
    public Task Maps_All_Methods(MethodResult result) => VerifyMethod(
                                                             result,
                                                             new Mapper(),
                                                             _fakeTimeProvider.GetUtcNow(),
                                                             _fakeTimeProvider.GetUtcNow().UtcDateTime,
                                                             Instant.FromDateTimeOffset(_fakeTimeProvider.GetUtcNow())
                                                         )
                                                        .UseParameters(result.ToString());

    private readonly FakeTimeProvider _fakeTimeProvider = new();

    [Mapper]
    [PublicAPI]
    [UseStaticMapper(typeof(DateTimeMapper))]
    [UseStaticMapper(typeof(NodaTimeMapper))]
    [UseStaticMapper(typeof(NodaTimeDateTimeMapper))]
    private partial class Mapper
    {
        public partial Foo1 MapFoo1(Foo2 source);
        public partial Foo1 MapFoo1(Foo3 source);
        public partial Foo1 MapFoo1(Foo4 source);
        public partial Foo1 MapFoo1(Foo5 source);
        public partial Foo1 MapFoo1(Foo6 source);

        public partial Foo2 MapFoo2(Foo1 source);
        public partial Foo2 MapFoo2(Foo3 source);
        public partial Foo2 MapFoo2(Foo4 source);
        public partial Foo2 MapFoo2(Foo5 source);
        public partial Foo2 MapFoo2(Foo6 source);

        public partial Foo3 MapFoo3(Foo1 source);
        public partial Foo3 MapFoo3(Foo2 source);
        public partial Foo3 MapFoo3(Foo4 source);
        public partial Foo3 MapFoo3(Foo5 source);
        public partial Foo3 MapFoo3(Foo6 source);

        public partial Foo4 MapFoo4(Foo1 source);
        public partial Foo4 MapFoo4(Foo2 source);
        public partial Foo4 MapFoo4(Foo3 source);
        public partial Foo4 MapFoo4(Foo5 source);
        public partial Foo4 MapFoo4(Foo6 source);

        public partial Foo5 MapFoo5(Foo1 source);
        public partial Foo5 MapFoo5(Foo2 source);
        public partial Foo5 MapFoo5(Foo3 source);
        public partial Foo5 MapFoo5(Foo4 source);
        public partial Foo5 MapFoo5(Foo6 source);

        public partial Foo6 MapFoo6(Foo1 source);
        public partial Foo6 MapFoo6(Foo2 source);
        public partial Foo6 MapFoo6(Foo3 source);
        public partial Foo6 MapFoo6(Foo4 source);
        public partial Foo6 MapFoo6(Foo5 source);
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
}
