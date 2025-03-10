using Microsoft.Extensions.Time.Testing;

using NodaTime;

using Riok.Mapperly.Abstractions;

using Rocket.Surgery.LaunchPad.Mapping;
using NodaTimeMapper = Rocket.Surgery.LaunchPad.Mapping.NodaTimeMapper;

#pragma warning disable CA1034 // Nested types should not be visible

namespace Extensions.Tests.Mapping;

public partial class LocalDateTimeTests(ITestContextAccessor testContext) : MapperTestBase(testContext)
{
    [Theory]
    [MapperData<Mapper>]
    public Task Maps_All_Methods(MethodResult result) => VerifyMethod(
                                                             result,
                                                             new Mapper(),
                                                             _fakeTimeProvider.GetLocalNow().DateTime,
                                                             LocalDateTime.FromDateTime(_fakeTimeProvider.GetLocalNow().DateTime)
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
        public LocalDateTime Bar { get; set; }
    }

    private class Foo2
    {
        public LocalDateTime? Bar { get; set; }
    }

    private class Foo3
    {
        public DateTime Bar { get; set; }
    }

    private class Foo4
    {
        public DateTime? Bar { get; set; }
    }
}
