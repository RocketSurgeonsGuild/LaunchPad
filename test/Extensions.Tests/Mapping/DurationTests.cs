using NodaTime;
using Riok.Mapperly.Abstractions;
using Rocket.Surgery.LaunchPad.Mapping.Profiles;

#pragma warning disable CA1034 // Nested types should not be visible

namespace Extensions.Tests.Mapping;

public partial class DurationTests(ITestOutputHelper testOutputHelper) : MapperTestBase(testOutputHelper)
{
    [Theory]
    [MapperData<Mapper>]
    public Task Maps_All_Methods(MethodResult result)
    {
        return VerifyMethod(result, new Mapper(), TimeSpan.FromHours(1), Duration.FromMinutes(44))
              .UseParameters(result.ToString())
              .HashParameters();
    }

    [Mapper]
    [PublicAPI]
    [UseStaticMapper(typeof(NodaTimeMapper))]
    private partial class Mapper
    {
        public partial Foo1 MapFoo1(Foo2 source);
        public partial Foo1 MapFoo1(Foo3 source);
        public partial Foo1 MapFoo1(Foo4 source);

        public partial Foo2 MapFoo2(Foo1 source);
        public partial Foo2 MapFoo2(Foo3 source);
        public partial Foo2 MapFoo2(Foo4 source);

        public partial Foo3 MapFoo3(Foo1 source);
        public partial Foo3 MapFoo3(Foo2 source);
        public partial Foo3 MapFoo3(Foo4 source);

        public partial Foo4 MapFoo4(Foo1 source);
        public partial Foo4 MapFoo4(Foo2 source);
        public partial Foo4 MapFoo4(Foo3 source);
    }

    private class Foo1
    {
        public Duration Bar { get; set; }
    }

    private class Foo2
    {
        public Duration? Bar { get; set; }
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
