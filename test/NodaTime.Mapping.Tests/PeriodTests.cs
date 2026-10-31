using Microsoft.Extensions.Time.Testing;

using Riok.Mapperly.Abstractions;

using Rocket.Surgery.LaunchPad.Mapping;
using NodaTimeMapper = Rocket.Surgery.LaunchPad.Mapping.NodaTimeMapper;

namespace NodaTime.Mapping.Tests;

public partial class PeriodTests() : MapperTestBase()
{
    [Test]
    [MethodDataSource(nameof(Methods))]
    public Task Maps_All_Methods(MethodResult result) => VerifyMethod(
                                                             result,
                                                             new Mapper(),
                                                             Period.FromMonths(10),
                                                             "P5M"
                                                         )
                                                        .UseParameters(result.ToString());

    private readonly FakeTimeProvider _fakeTimeProvider = new();

    public static IEnumerable<Func<MethodResult>> Methods() => MapperData.Methods<Mapper>();

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
