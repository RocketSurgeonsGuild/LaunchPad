using System.Reflection;
using NodaTime;
using Riok.Mapperly.Abstractions;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.Mapping.Profiles;

#pragma warning disable CA1034 // Nested types should not be visible

namespace Extensions.Tests.Mapping;

public partial class DurationTests(ITestOutputHelper testOutputHelper) : AutoFakeTest(testOutputHelper)
{
    private class Foo1
    {
        public Duration Bar { get; set; }
    }

    private class Foo3
    {
        public TimeSpan Bar { get; set; }
    }

    [Theory, MapperData<Mapper>]
    public Task TestsMapper(MethodResult result)
    {
        return Verify(result.Map(
            new Mapper(),
            TimeSpan.FromHours(2),
            Duration.FromMinutes(44)
        )).UseHashedParameters(result.ToString());
    }

    [Mapper]
    [UseStaticMapper(typeof(NodaTimeMapper))]
    private partial class Mapper
    {
        public  partial Foo1 Map(Foo3 source);
        public  partial Foo3 Map(Foo1 source);
    }
}
