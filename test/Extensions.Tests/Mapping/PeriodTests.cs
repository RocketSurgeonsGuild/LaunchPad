using Microsoft.Extensions.Time.Testing;
using NodaTime;
using Riok.Mapperly.Abstractions;
using Rocket.Surgery.LaunchPad.Mapping;
using Rocket.Surgery.LaunchPad.Mapping.Profiles;

namespace Extensions.Tests.Mapping;

public partial class PeriodTests(ITestOutputHelper testOutputHelper) : MapperTestBase(testOutputHelper)
{
    private FakeTimeProvider _fakeTimeProvider = new();

    [Theory]
    [MapperData<Mapper>]
    public Task Maps_All_Methods(MethodResult result)
    {
        return VerifyMethod(
                   result,
                   new Mapper(),
                   Period.FromMonths(10),
                   "P5M"
               )
              .UseParameters(result.ToString())
              .HashParameters();
    }

    [Mapper]
    [UseStaticMapper(typeof(NodaTimeMapper))]
    [UseStaticMapper(typeof(NodaTimeDateTimeMapper))]
    private partial class Mapper
    {
        public partial Foo1 MapFoo1(Foo3 foo);
        public partial Foo3 MapFoo3(Foo1 foo);
    }

    private class Foo1
    {
        public Period? Bar { get; set; }
    }

    private class Foo3
    {
        public string? Bar { get; set; }
    }
}
