using Microsoft.Extensions.Time.Testing;
using NodaTime;
using Riok.Mapperly.Abstractions;
using Rocket.Surgery.LaunchPad.Mapping.Profiles;

namespace Extensions.Tests.Mapping;

public partial class LocalTimeTests(ITestOutputHelper testOutputHelper) : MapperTestBase(testOutputHelper)
{
    private FakeTimeProvider _fakeTimeProvider = new();

    [Theory]
    [MapperData<Mapper>]
    public Task Maps_All_Methods(MethodResult result)
    {
        return VerifyMethod(
                   result,
                   new Mapper(),
                   _fakeTimeProvider.GetLocalNow().DateTime,
                   TimeOnly.FromDateTime(_fakeTimeProvider.GetLocalNow().DateTime),
                   LocalTime.FromTimeOnly(TimeOnly.FromDateTime(_fakeTimeProvider.GetLocalNow().DateTime))
               )
              .UseParameters(result.ToString())
              .HashParameters();
    }

    [Mapper]
    [UseStaticMapper(typeof(NodaTimeMapper))]
    private partial class Mapper
    {
        public partial Foo1 MapFoo1(Foo2 foo);
        public partial Foo1 MapFoo1(Foo5 foo);
        public partial Foo1 MapFoo1(Foo6 foo);

        public partial Foo2 MapFoo2(Foo1 foo);
        public partial Foo2 MapFoo2(Foo5 foo);
        public partial Foo2 MapFoo2(Foo6 foo);

        public partial Foo5 MapFoo5(Foo1 foo);
        public partial Foo5 MapFoo5(Foo2 foo);
        public partial Foo5 MapFoo5(Foo6 foo);

        public partial Foo6 MapFoo6(Foo1 foo);
        public partial Foo6 MapFoo6(Foo2 foo);
        public partial Foo6 MapFoo6(Foo5 foo);
    }

    private class Foo1
    {
        public LocalTime Bar { get; set; }
    }

    private class Foo2
    {
        public LocalTime? Bar { get; set; }
    }

    private class Foo5
    {
        public TimeOnly Bar { get; set; }
    }

    private class Foo6
    {
        public TimeOnly? Bar { get; set; }
    }
}
