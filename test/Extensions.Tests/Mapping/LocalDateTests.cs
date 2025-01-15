using Microsoft.Extensions.Time.Testing;
using NodaTime;
using Riok.Mapperly.Abstractions;
using NodaTimeMapper = Rocket.Surgery.LaunchPad.Mapping.NodaTimeMapper;

namespace Extensions.Tests.Mapping;

public partial class LocalDateTests(ITestOutputHelper testOutputHelper) : MapperTestBase(testOutputHelper)
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
                   DateOnly.FromDateTime(_fakeTimeProvider.GetLocalNow().DateTime),
                   LocalDate.FromDateTime(_fakeTimeProvider.GetLocalNow().DateTime)
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
        public LocalDate Bar { get; set; }
    }

    private class Foo2
    {
        public LocalDate? Bar { get; set; }
    }

    private class Foo5
    {
        public DateOnly Bar { get; set; }
    }

    private class Foo6
    {
        public DateOnly? Bar { get; set; }
    }
}
