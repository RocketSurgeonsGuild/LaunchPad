using Rocket.Surgery.Extensions.Testing;

namespace Extensions.Tests.Mapping.Helpers;

public abstract class MapperTestBase(ITestOutputHelper testOutputHelper) : AutoFakeTest(testOutputHelper)
{
    protected SettingsTask VerifyMethod(MethodResult result, object mapper, params object[] instances) =>
        Verify(result.Map(mapper, instances)).UseHashedParameters(result.ToString());
    protected SettingsTask VerifyEachMethod(MethodResult result, object mapper, params object[] instances) =>
        Verify(result.MapEach(mapper, instances)).UseHashedParameters(result.ToString());
}
